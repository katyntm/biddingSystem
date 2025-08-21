using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CarAuction.Infrastructure.Jobs
{
    public class AuctionEndJob : IJob
    {
        private readonly ILogger<AuctionEndJob> _logger;
        private readonly MoveNextSession _moveSession;
        public AuctionEndJob(ILogger<AuctionEndJob> logger, MoveNextSession moveSession)
        {
            _moveSession = moveSession;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Auction end Job started at {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString("dd/MM/yyyy HH:mm:ss")}");
            await _moveSession.MoveUnsoldVehicleAsync();
            _logger.LogInformation($"Auction end Job finished at {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString("dd/MM/yyyy HH:mm:ss")}");
        }
    }
}
