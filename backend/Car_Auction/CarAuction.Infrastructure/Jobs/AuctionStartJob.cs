using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CarAuction.Infrastructure.Jobs
{
    public class AuctionStartJob : IJob
    {
        private readonly ImportAuctionSetting _setting;
        private readonly LoadAuctionVehicle _auctionVehicle;
        private readonly ILogger<AuctionStartJob> _logger;
        public AuctionStartJob(ImportAuctionSetting setting, LoadAuctionVehicle auctionVehicle, ILogger<AuctionStartJob> logger)
        {
            _auctionVehicle = auctionVehicle;
            _setting = setting;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Auction start Job started at {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString("dd/MM/yyyy HH:mm:ss")}");
           
            await _setting.ImportAuctionSettingAsync();
           
            await _auctionVehicle.LoadAuctionVehiclesAsync();
           
            _logger.LogInformation($"Auction start Job finished at {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString("dd/MM/yyyy HH:mm:ss")}");
        }
    }
}
