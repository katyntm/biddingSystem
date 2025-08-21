using CarAuction.Application.Common.Constants;
using CarAuction.Infrastructure.Jobs;
using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace CarAuction.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleImportController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<VehicleImportController> _logger;
        private readonly VehicleCsvImportService _csvImportService;
        private readonly VehicleImageImportService _imageImportService;

        public VehicleImportController(
            ISchedulerFactory schedulerFactory,
            ILogger<VehicleImportController> logger,
            VehicleCsvImportService csvImportService,
            VehicleImageImportService imageImportService)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
            _csvImportService = csvImportService;
            _imageImportService = imageImportService;
        }

        [HttpPost("import-csv")]
        public async Task<IActionResult> ImportVehicleCsv()
        {
            try
            {
                _logger.LogInformation("Manually triggering Vehicle CSV import");
                await _csvImportService.ImportVehiclesFromCsvAsync();
                return Ok("Vehicle CSV import completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during manual Vehicle CSV import");
                return StatusCode(500, "Error during CSV import: " + ex.Message);
            }
        }

        [HttpPost("import-images")]
        public async Task<IActionResult> ImportVehicleImages()
        {
            try
            {
                _logger.LogInformation("Manually triggering Vehicle image import");
                await _imageImportService.ImportVehicleImagesAsync();
                return Ok("Vehicle image import completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during manual Vehicle image import");
                return StatusCode(500, "Error during image import: " + ex.Message);
            }
        }
    }
}