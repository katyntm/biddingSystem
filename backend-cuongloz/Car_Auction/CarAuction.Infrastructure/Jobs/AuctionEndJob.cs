using CarAuction.Infrastructure.Services;
using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CarAuction.Infrastructure.Jobs
{
    public class AuctionEndJob : IJob
    {
        private readonly ILogger<AuctionEndJob> _logger;
        private readonly moveNextSession _moveSession;
        public AuctionEndJob(ILogger<AuctionEndJob> logger, moveNextSession moveSession)
        {
            _moveSession = moveSession;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Auction end Job started at {DateTime.UtcNow}");
            await _moveSession.moveUnsoldVehicleAsync();
            _logger.LogInformation($"Auction end Job finished at {DateTime.UtcNow}");
        }
    }
}
