using CarAuction.Domain.Entities;
using CarAuction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CarAuction.Infrastructure.Services
{
    public class ImportAuctionSetting
    {
        private readonly CarAuctionDbContext _db;
        private readonly ILogger<ImportAuctionSetting> _logger;

        public ImportAuctionSetting(CarAuctionDbContext db, ILogger<ImportAuctionSetting> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task ImportFromJsonAsync()
        {
            try
            {
                var filePath = "C:\\Users\\CuongPC10\\Desktop\\OJT_Training\\backend\\Car_Auction\\CarAuction.Infrastructure\\LoadData\\auctionSetting.json";
                var json = await File.ReadAllTextAsync(filePath);
                var document = JsonDocument.Parse(json);
                var root = document.RootElement;

                var auctionSession = root.GetProperty("auctionSession");
                var startTime = auctionSession.GetProperty("startTime").GetDateTime();
                var endTime = auctionSession.GetProperty("endTime").GetDateTime();

                // ===== SALE CHANNELS =====
                var saleChannelsArray = root.GetProperty("saleChannel");
                foreach (var saleChannelItem in saleChannelsArray.EnumerateArray())
                {
                    var name = saleChannelItem.GetProperty("name").GetString();
                    var pricePercentage = saleChannelItem.GetProperty("pricePercentage").GetDecimal();
                    var buyItNowPercentage = saleChannelItem.GetProperty("buyItNowPercentage").GetDecimal();

                    var existing = await _db.SaleChannels.FirstOrDefaultAsync(sc => sc.Name == name);
                    if (existing == null)
                    {
                        _db.SaleChannels.Add(new SaleChannel
                        {
                            Name = name,
                            PricePercentage = pricePercentage,
                            BuyItNowPercentage = buyItNowPercentage
                        });
                    }
                    else
                    {
                        existing.PricePercentage = pricePercentage;
                        existing.BuyItNowPercentage = buyItNowPercentage;
                        _db.SaleChannels.Update(existing);
                    }
                }
                await _db.SaveChangesAsync();

                // ===== CLEAR OLD TACTICS / RELATED =====
                var oldTactics = await _db.Tactics.Include(t => t.Criterias).Include(t => t.Steps).ToListAsync();
                foreach (var oldTactic in oldTactics)
                {
                    var stepIds = await _db.Steps.Where(s => s.TacticId == oldTactic.Id).Select(s => s.Id).ToListAsync();
                    var auctionVehicles = await _db.AuctionVehicles.Where(av => stepIds.Contains(av.StepId)).ToListAsync();
                    _db.AuctionVehicles.RemoveRange(auctionVehicles);

                    _db.Criteria.RemoveRange(oldTactic.Criterias);
                    _db.Steps.RemoveRange(oldTactic.Steps);
                }
                _db.Tactics.RemoveRange(oldTactics);
                await _db.SaveChangesAsync();

                // ===== IMPORT TACTICS / CRITERIA / STEPS / AUCTION VEHICLES =====
                var tacticsArray = root.GetProperty("tactics");
                foreach (var tacticItem in tacticsArray.EnumerateArray())
                {
                    var tactic = new Tactic
                    {
                        Name = tacticItem.GetProperty("name").GetString(),
                        CreatedAt = DateTime.UtcNow
                    };
                    _db.Tactics.Add(tactic);
                    await _db.SaveChangesAsync();

                    var tacticId = tactic.Id;

                    // Criteria
                    var criteriaArray = tacticItem.GetProperty("criteria");
                    foreach (var c in criteriaArray.EnumerateArray())
                    {
                        _db.Criteria.Add(new Criteria
                        {
                            TacticId = tacticId,
                            FieldName = c.GetProperty("fieldName").GetString(),
                            Operator = c.GetProperty("operator").GetString(),
                            Value = c.GetProperty("value").GetString()
                        });
                    }

                    // Steps
                    var stepsArray = tacticItem.GetProperty("steps");
                    foreach (var s in stepsArray.EnumerateArray())
                    {
                        var stepNumber = s.GetProperty("stepNumber").GetInt32();
                        var saleChannelName = s.GetProperty("saleChannelName").GetString();
                        var saleChannel = await _db.SaleChannels.FirstOrDefaultAsync(sc => sc.Name == saleChannelName);

                        if (saleChannel == null)
                        {
                            _logger.LogWarning($"SaleChannel '{saleChannelName}' not found for step.");
                            continue;
                        }

                        var step = new Step
                        {
                            TacticId = tacticId,
                            StepNumber = stepNumber,
                            SaleChannelId = saleChannel.Id
                        };
                        _db.Steps.Add(step);
                        await _db.SaveChangesAsync();

                        // TODO: Filter vehicle logic here (chưa xử lý ở đây do chưa có CSV parsing)
                        // => For now, giả sử tạo AuctionVehicle rỗng
                        //var dummyVehicleId = Guid.NewGuid().ToString(); // Giả
                        //_db.AuctionVehicles.Add(new AuctionVehicle
                        //{
                        //    Id = Guid.NewGuid(),
                        //    VehicleId = dummyVehicleId,
                        //    TacticId = tacticId,
                        //    StepId = step.Id,
                        //    CurrentPrice = 0,
                        //    BuyItNowPrice = null,
                        //    WinnerUserId = null,
                        //    IsSold = false,
                        //    CreatedAt = DateTime.UtcNow,
                        //    StartTime = startTime,
                        //    EndTime = endTime
                        //});
                    }

                    await _db.SaveChangesAsync();
                }

                _logger.LogInformation("Import tactic setting from JSON completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing tactic JSON.");
                throw;
            }
        }
    }
}