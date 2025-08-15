using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _logger.LogInformation($"Auction start Job started at {DateTime.UtcNow}");
            await _setting.ImportAuctionSettingAsync();
            await _auctionVehicle.LoadAuctionVehiclesAsync();
            _logger.LogInformation($"Auction start Job finished at {DateTime.UtcNow}");
        }
    }
}
