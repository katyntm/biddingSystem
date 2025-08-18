using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CarAuction.Infrastructure.Jobs
{
    public class VehicleImportJob : IJob
    {
        private readonly CsvImportService _csvImportService;
        private readonly ImportAuctionSetting _tacticImportService;
        private readonly ImageImportService _imageImportService;
        private readonly LoadAuctionVehicle _loadAuctionVehicle;
        private readonly ILogger<VehicleImportJob> _logger;

        public VehicleImportJob(
            CsvImportService csvImportService,
            ImportAuctionSetting tacticImportService,
            ImageImportService imageImportService,
            LoadAuctionVehicle loadAuctionVehicle,
            ILogger<VehicleImportJob> logger)
        {
            _csvImportService = csvImportService;
            _tacticImportService = tacticImportService;
            _imageImportService = imageImportService;
            _loadAuctionVehicle = loadAuctionVehicle;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Vehicle Import Job started at {DateTime.UtcNow}");

            try
            {
                // 1. CSV Import - Parse vehicle CSV and import to database
                _logger.LogInformation("Starting CSV import...");
                await _csvImportService.ImportVehiclesFromCsvAsync();

                // 2. Tactic Import - Keep original logic from backend-cuongloz
                _logger.LogInformation("Starting tactic import...");
                await _tacticImportService.ImportAuctionSettingAsync();

                // 3. Load auction vehicles using existing logic
                _logger.LogInformation("Loading auction vehicles...");
                await _loadAuctionVehicle.LoadAuctionVehiclesAsync();

                // 4. Image Import - Process ZIP files and associate with vehicles
                _logger.LogInformation("Starting image import...");
                await _imageImportService.ImportVehicleImagesAsync();

                _logger.LogInformation($"Vehicle Import Job completed successfully at {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Vehicle Import Job execution");
                throw;
            }
        }
    }
}