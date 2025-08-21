using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.Extensions.Logging;
using Quartz;

namespace CarAuction.Infrastructure.Jobs
{
    public class VehicleImportJob : IJob
    {
        private readonly VehicleCsvImportService _csvImportService;
        private readonly VehicleImageImportService _imageImportService;
        private readonly ILogger<VehicleImportJob> _logger;

        public VehicleImportJob(
        VehicleCsvImportService csvImportService,
        VehicleImageImportService imageImportService,
        ILogger<VehicleImportJob> logger)
        {
            _csvImportService = csvImportService;
            _imageImportService = imageImportService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Vehicle Import Job started at {DateTime.UtcNow}");

            try
            {
                // Step 1: Import vehicles from CSV
                _logger.LogInformation("Starting CSV import...");
                await _csvImportService.ImportVehiclesFromCsvAsync();

                // Step 2: Import vehicle images from ZIP files
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