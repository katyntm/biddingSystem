using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using CarAuction.Infrastructure.Services;
using Quartz;

namespace CarAuction.Infrastructure.Jobs
{
    public class AuctionEndJob : IJob
    {
        private readonly ILogger<AuctionEndJob> _logger;
    private readonly AuctionEndService _auctionEndService;
    private readonly MoveNextSession _moveSession;

        public AuctionEndJob(ILogger<AuctionEndJob> logger, MoveNextSession moveSession, AuctionEndService auctionEndService)
        {
            _moveSession = moveSession;
            _logger = logger;
            _auctionEndService = auctionEndService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Auction end Job started at {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString("dd/MM/yyyy HH:mm:ss")}");

            await _auctionEndService.ProcessEndedAuctionsAsync();

            await _moveSession.MoveUnsoldVehicleAsync();

            await UpdateAuctionSessionDateAsync();

            _logger.LogInformation($"Auction end Job finished at {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString("dd/MM/yyyy HH:mm:ss")}");
        }

        private async Task UpdateAuctionSessionDateAsync()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                 "LoadData", "auctionSetting.json");
                _logger.LogInformation($"Updating auction session date from file: {filePath}");
                string jsonContent = await File.ReadAllTextAsync(filePath);
                var jObject = JObject.Parse(jsonContent);

                DateTime currentStartTime = jObject["auctionSession"]["startTime"].Value<DateTime>();
                DateTime currentEndTime = jObject["auctionSession"]["endTime"].Value<DateTime>();

                DateTime newStartTime = currentStartTime.AddDays(1);
                DateTime newEndTime = currentEndTime.AddDays(1);

                _logger.LogInformation($"Updateing Current Start Time: {currentStartTime}, New Start Time: {newStartTime}");
                _logger.LogInformation($"Updateing Current End Time: {currentEndTime}, New End Time: {newEndTime}");

                jObject["auctionSession"]["startTime"] = newStartTime.ToString("yyyy-MM-ddTHH:mm:ss");
                jObject["auctionSession"]["endTime"] = newEndTime.ToString("yyyy-MM-ddTHH:mm:ss");

                string updatedJson = jObject.ToString(Newtonsoft.Json.Formatting.Indented);

                await File.WriteAllTextAsync(filePath, updatedJson);

                _logger.LogInformation("Auction session date updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating auction session date.");
            }
        }
    }
}
