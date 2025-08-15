using CarAuction.Domain.Entities;
using CarAuction.Infrastructure.Mock;
using CarAuction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CarAuction.Infrastructure.Services.CronJobService
{
    public class LoadAuctionVehicle
    {
        private readonly CarAuctionDbContext _db;
        private readonly ILogger<LoadAuctionVehicle> _logger;

        public LoadAuctionVehicle(CarAuctionDbContext db, ILogger<LoadAuctionVehicle> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<List<Vehicle>> LoadVehicleInventoryAsync()
        {
            var filePath = "C:\\Users\\CuongPC10\\Desktop\\OJT_Training\\backend\\Car_Auction\\CarAuction.Infrastructure\\LoadData\\mockVehiclesTable.json";
            if (!File.Exists(filePath))
            {

                _logger.LogWarning($"File JSON not found: {filePath}");
                return new List<Vehicle>();
            }

            try
            {
                var jsonContent = await File.ReadAllTextAsync(filePath);
                var vehicles = JsonSerializer.Deserialize<List<Vehicle>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _logger.LogInformation("Loading vehicles successful");
                return vehicles ?? new List<Vehicle>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading or deserializing vehicle JSON file.");
                return new List<Vehicle>();
            }
        }

        private int GetVehicleScore(Vehicle vehicle, List<Criteria> criterias)
        {
            int score = 0;

            foreach (var c in criterias)
            {
                var values = c.Value.Split(',')
                                    .Select(v => v.Trim())
                                    .ToList();

                switch (c.FieldName)
                {
                    case "Make":
                        foreach (var v in values)
                            if (vehicle.Make == v)
                                score++;
                        break;

                    case "ModelYear":
                        foreach (var v in values)
                            if (vehicle.ModelYear == int.Parse(v))
                                score++;
                        break;

                    case "FuelType":
                        foreach (var v in values)
                            if (vehicle.FuelType == v)
                                score++;
                        break;

                    case "Grade":
                        foreach (var v in values)
                        {
                            var num = decimal.Parse(v);

                            if (c.Operator == ">=" && vehicle.Grade >= num)
                                score++;
                            else if (c.Operator == "<=" && vehicle.Grade <= num)
                                score++;
                            else if (c.Operator == "=" && vehicle.Grade == num)
                                score++;
                            else if (c.Operator == ">" && vehicle.Grade > num)
                                score++;
                            else if (c.Operator == "<" && vehicle.Grade < num)
                                score++;
                        }
                        break;
                }
            }

            return score;
        }
        public async Task LoadAuctionVehiclesAsync()
        {
            var filePath = "C:\\Users\\CuongPC10\\Desktop\\OJT_Training\\backend\\Car_Auction\\CarAuction.Infrastructure\\LoadData\\auctionSetting.json";
            var json = await File.ReadAllTextAsync(filePath);
            var root = JsonDocument.Parse(json).RootElement;

            var startTime = root.GetProperty("auctionSession").GetProperty("startTime").GetDateTime();
            var endTime = root.GetProperty("auctionSession").GetProperty("endTime").GetDateTime();

            var allVehicles = await LoadVehicleInventoryAsync();
        
            var tactics = await _db.Tactics.Include(t => t.Criterias).Include(t => t.Steps).ToListAsync();

            foreach (var tactic in tactics)
            {
                var criterias = tactic.Criterias.ToList();

                var scoredVehicles = allVehicles
                    .Select(v => new { Vehicle = v, Score = GetVehicleScore(v, criterias) })
                    .Where(x => x.Score >= 4)
                    .ToList();

                List<Vehicle> finalVehicles = new();
                if (scoredVehicles.Any())
                {
                    int maxScore = scoredVehicles.Max(x => x.Score);
                    finalVehicles.AddRange(scoredVehicles.Select(x => x.Vehicle));
                    var topVehicles = scoredVehicles.Where(x => x.Score == maxScore).ToList();
                    if (topVehicles.Count > 1)
                    {
                        var chosen = topVehicles.OrderBy(x => x.Vehicle.Id).First().Vehicle;
                        finalVehicles.RemoveAll(v => topVehicles.Select(tv => tv.Vehicle.Id).Contains(v.Id));
                        finalVehicles.Add(chosen);
                    }
                }

                var step1 = tactic.Steps.OrderBy(s => s.StepNumber).FirstOrDefault();
                if (step1 != null)
                {
                    var saleChannel = await _db.SaleChannels.FirstOrDefaultAsync(sc => sc.Id == step1.SaleChannelId);
                    foreach (var vehicle in finalVehicles)
                    {

                        var existingVehicleIds = await _db.AuctionVehicles
                            .Select(av => av.VehicleId)
                            .ToListAsync();

                        bool exists = allVehicles
                            .Where(v => existingVehicleIds.Contains(v.Id))
                            .Any(v => v.VIN == vehicle.VIN);

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
                            StepId = step1.Id,
                            CurrentPrice = vehicle.Price * saleChannel.PricePercentage,
                            BuyItNowPrice = vehicle.Price * saleChannel.BuyItNowPercentage,
                            WinnerUserId = null,
                            IsSold = false,
                            CreatedAt = DateTime.UtcNow,
                            StartTime = startTime,
                            EndTime = endTime
                        });
                    }
                }
            }
            await _db.SaveChangesAsync();
        }
    }
}
