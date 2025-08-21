using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces.UnitOfWork;
using CarAuction.Infrastructure.Options;
using CarAuction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CarAuction.Infrastructure.Services.CronJobService
{
    public class LoadAuctionVehicle
    {
        private readonly CarAuctionDbContext _db;
        private readonly ILogger<LoadAuctionVehicle> _logger;
        private readonly AuctionSettingOptions _auctionSetting;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _uow;

        public LoadAuctionVehicle(CarAuctionDbContext db, ILogger<LoadAuctionVehicle> logger, IOptions<AuctionSettingOptions> auctionSetting, IConfiguration config, IUnitOfWork uow)
        {
            _db = db;
            _logger = logger;
            _auctionSetting = auctionSetting.Value;
            _config = config;
            _uow = uow;
        }
        public async Task<List<Vehicle>> LoadVehicleInventoryAsync()
        {
            try
            {
                var vehicles = (await _uow.Vehicles.GetAllAsync()).ToList();
                _logger.LogInformation("Loading vehicles successful");
                return vehicles ?? new List<Vehicle>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading or deserializing vehicle JSON file.");
                return new List<Vehicle>();
            }
        }


        private int GetVehicleScore(Vehicle vehicle, List<Criteria> criteriaList)
        {
            int score = 0;

            var groupedCriteria = criteriaList.GroupBy(c => c.FieldName);

            foreach (var criteriaGroup in groupedCriteria)
            {
                //reflection
                var propertyInfo = typeof(Vehicle).GetProperty(criteriaGroup.Key);
                if (propertyInfo == null) continue;

                var vehicleValue = propertyInfo.GetValue(vehicle)?.ToString();
                if (string.IsNullOrEmpty(vehicleValue)) continue;

                bool matched = criteriaGroup.Any(criteria => IsCriteriaMatched(vehicleValue, criteria));
                if (matched)
                {
                    score++;
                }
            }

            return score;
        }

        private bool IsCriteriaMatched(string vehicleValue, Criteria criteria)
        {
            var criteriaValues = criteria.Value
                .Split(',')
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrEmpty(v))
                .ToList();

            foreach (var criteriaValue in criteriaValues)
            {
                if (IsNumber(vehicleValue) && IsNumber(criteriaValue))
                {
                    if (CheckNumeric(vehicleValue, criteriaValue, criteria.Operator))
                        return true;
                }
                else
                {
                    if (vehicleValue.Equals(criteriaValue, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }


        private bool IsNumber(string input)
        {
            try
            {
                decimal.Parse(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool CheckNumeric(string vehicleValue, string criteriaValue, string operatorSymbol)
        {
            decimal vehicleNumber = decimal.Parse(vehicleValue);
            decimal criteriaNumber = decimal.Parse(criteriaValue);

            return operatorSymbol switch
            {
                ">=" => vehicleNumber >= criteriaNumber,
                "<=" => vehicleNumber <= criteriaNumber,
                "=" => vehicleNumber == criteriaNumber,
                ">" => vehicleNumber > criteriaNumber,
                "<" => vehicleNumber < criteriaNumber,
                _ => false
            };
        }

        public async Task LoadAuctionVehiclesAsync()
        {
            var allVehicles = await LoadVehicleInventoryAsync();
            var tactics = await _db.Tactics.Include(t => t.Criterias).Include(t => t.Steps).ToListAsync();

            foreach (var tactic in tactics)
            {
                var criterias = tactic.Criterias.ToList();
                var count = criterias.Select(c => c.FieldName).Distinct().Count();

                var makeCriteria = criterias
                    .Where(c => c.FieldName == "Make")
                    .SelectMany(c => c.Value.Split(',').Select(v => v.Trim()))
                    .ToList();

                var tacticVehicles = allVehicles;
                if (makeCriteria.Any())
                {
                    tacticVehicles = allVehicles
                        .Where(v => makeCriteria.Contains(v.Make, StringComparer.OrdinalIgnoreCase))
                        .ToList();
                }

                var scoredVehicles = tacticVehicles
                    .Select(v => new { Vehicle = v, Score = GetVehicleScore(v, criterias) })
                    .Where(x => x.Score == count)
                    .ToList();

                var step = tactic.Steps.OrderBy(s => s.StepNumber).FirstOrDefault();
                if (step != null)
                {
                    var saleChannel = await _db.SaleChannels.FirstOrDefaultAsync(sc => sc.Id == step.SaleChannelId);

                    foreach (var x in scoredVehicles)
                    {
                        var vehicle = x.Vehicle;

                        bool exists = await _db.AuctionVehicles.AnyAsync(av => av.VehicleId == vehicle.Id);
                        if (exists)
                        {
                            _logger.LogInformation($"Vehicle with VIN {vehicle.VIN} already exists in auction, skipping.");
                            continue;
                        }

                        _db.AuctionVehicles.Add(new AuctionVehicle
                        {
                            Id = Guid.NewGuid(),
                            VehicleId = vehicle.Id,
                            TacticId = tactic.Id,
                            StepId = step.Id,
                            CurrentPrice = vehicle.Price * saleChannel.PricePercentage,
                            BuyItNowPrice = vehicle.Price * saleChannel.BuyItNowPercentage,
                            WinnerUserId = null,
                            IsSold = false,
                            CreatedAt = DateTime.UtcNow,
                            StartTime = _auctionSetting.AuctionSession.StartTime,
                            EndTime = _auctionSetting.AuctionSession.EndTime
                        });
                    }
                }
            }

            await _db.SaveChangesAsync();
        }

    }
}
