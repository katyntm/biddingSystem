using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CarAuction.Infrastructure.Jobs
{
    public class AuctionJob : IJob
    {
        private readonly ImportAuctionSetting _setting;
        private readonly LoadAuctionVehicle _auctionVehicle;
        private readonly ILogger<AuctionJob> _logger;
        public AuctionJob(ImportAuctionSetting setting, LoadAuctionVehicle auctionVehicle, ILogger<AuctionJob> logger)
        {
            _auctionVehicle = auctionVehicle;
            _setting = setting;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Auction Job started at {DateTime.UtcNow}");
            await _setting.ImportAuctionSettingAsync();
            await _auctionVehicle.LoadAuctionVehiclesAsync();
            _logger.LogInformation($"Auction Job finished at {DateTime.UtcNow}");
        }
    }
}
