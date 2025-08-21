using CarAuction.Infrastructure.Options;
using CarAuction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CarAuction.Domain.Entities;

namespace CarAuction.Infrastructure.Services.CronJobService
{
    public class ImportAuctionSetting
    {
        private readonly CarAuctionDbContext _db;
        private readonly ILogger<ImportAuctionSetting> _logger;
        private readonly AuctionSettingOptions _auctionSetting;

        public ImportAuctionSetting(CarAuctionDbContext db, ILogger<ImportAuctionSetting> logger, IOptions<AuctionSettingOptions> auctionSetting)
        {
            _db = db;
            _logger = logger;
            _auctionSetting = auctionSetting.Value;
        }

        public async Task ImportAuctionSettingAsync()
        {
            // SALE CHANNEL
            foreach (var sc in _auctionSetting.SaleChannels)
            {
                var existing = await _db.SaleChannels.FirstOrDefaultAsync(x => x.Name == sc.Name);

                if (existing == null)
                {
                    _db.SaleChannels.Add(new SaleChannel
                    {
                        Id = Guid.NewGuid(),
                        Name = sc.Name,
                        PricePercentage = sc.PricePercentage,
                        BuyItNowPercentage = sc.BuyItNowPercentage
                    });
                }
                else
                {
                    if (existing.PricePercentage != sc.PricePercentage || existing.BuyItNowPercentage != sc.BuyItNowPercentage)
                    {
                        existing.PricePercentage = sc.PricePercentage;
                        existing.BuyItNowPercentage = sc.BuyItNowPercentage;
                        _db.SaleChannels.Update(existing);
                    }
                }
            }
            await _db.SaveChangesAsync();

            // TACTICS + CRITERIA + STEP
            foreach (var tacticItem in _auctionSetting.Tactics)
            {
                var tactic = await _db.Tactics.FirstOrDefaultAsync(t => t.Name == tacticItem.Name);

                if (tactic == null)
                {
                    tactic = new Tactic
                    {
                        Id = Guid.NewGuid(),
                        Name = tacticItem.Name,
                        CreatedAt = DateTime.UtcNow
                    };
                    _db.Tactics.Add(tactic);
                    await _db.SaveChangesAsync();
                }

                var tacticId = tactic.Id;

                // Criteria
                foreach (var c in tacticItem.Criterias)
                {
                    var existingCriteria = await _db.Criteria.FirstOrDefaultAsync(
                        x => x.TacticId == tacticId &&
                             x.FieldName == c.FieldName &&
                             x.Operator == c.Operator &&
                             x.Value == c.Value);

                    if (existingCriteria == null)
                    {
                        _db.Criteria.Add(new Criteria
                        {
                            Id = Guid.NewGuid(),
                            TacticId = tacticId,
                            FieldName = c.FieldName,
                            Operator = c.Operator,
                            Value = c.Value
                        });
                    }
                }

                // Step
                foreach (var s in tacticItem.Steps)
                {
                    var saleChannel = await _db.SaleChannels.FirstOrDefaultAsync(sc => sc.Name == s.SaleChannelName);
                    if (saleChannel == null) continue;

                    var existingStep = await _db.Steps.FirstOrDefaultAsync(
                        x => x.TacticId == tacticId &&
                             x.StepNumber == s.StepNumber &&
                             x.SaleChannelId == saleChannel.Id);

                    if (existingStep == null)
                    {
                        _db.Steps.Add(new Step
                        {
                            Id = Guid.NewGuid(),
                            TacticId = tacticId,
                            StepNumber = s.StepNumber,
                            SaleChannelId = saleChannel.Id
                        });
                    }
                }
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("ImportAuctionSetting completed successfully");
        }
    }
}