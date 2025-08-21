using CarAuction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarAuction.Infrastructure.Services.CronJobService
{
    public class MoveNextSession
    {
        private readonly CarAuctionDbContext _context;
        public MoveNextSession(CarAuctionDbContext context)
        {
            _context = context;
        }
        public async Task MoveUnsoldVehicleAsync()
        {
            var unsoldVehicles = await _context.AuctionVehicles
                .Where(x => !x.IsSold /*&& x.EndTime <= DateTime.UtcNow*/)
                .ToListAsync();

            foreach (var vehicle in unsoldVehicles)
            {
                var currentStep = await _context.Steps
                    .FirstOrDefaultAsync(s => s.Id == vehicle.StepId);

                if (currentStep == null)
                    continue;

                var nextStep = await _context.Steps
                    .Where(s => s.TacticId == vehicle.TacticId && s.StepNumber > currentStep.StepNumber)
                    .OrderBy(s => s.StepNumber)
                    .FirstOrDefaultAsync();

                if (nextStep != null)
                {
                    var saleChannel = await _context.SaleChannels
                        .FirstOrDefaultAsync(sc => sc.Id == nextStep.SaleChannelId);

                    if (saleChannel != null)
                    {
                        if (saleChannel.Name.Equals("Physical Auction", StringComparison.OrdinalIgnoreCase))
                        {
                            _context.AuctionVehicles.Remove(vehicle);
                            continue;
                        }

                        vehicle.StepId = nextStep.Id;
                        vehicle.CurrentPrice = vehicle.CurrentPrice * saleChannel.PricePercentage;
                        vehicle.BuyItNowPrice = vehicle.BuyItNowPrice * saleChannel.BuyItNowPercentage;

                        vehicle.StartTime = DateTime.UtcNow;
                        vehicle.EndTime = DateTime.UtcNow.AddHours(6);
                    }
                }
                else
                {
                    _context.AuctionVehicles.Remove(vehicle);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
