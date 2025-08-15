using CarAuction.Application.Common;
using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoadingDataController : ControllerBase
    {
        private readonly ImportAuctionSetting _auctionService;
        private readonly LoadVehicleInventory _vehicleLoadService;
        private readonly LoadAuctionVehicle _auctionLoadService;

        public LoadingDataController(ImportAuctionSetting importService, LoadVehicleInventory vehicleLoadService, LoadAuctionVehicle auctionLoadService)
        {
            _auctionService = importService;
            _vehicleLoadService = vehicleLoadService;
            _auctionLoadService = auctionLoadService;
        }

        [HttpPost("LoadVehicleInventory")]
        public async Task<IActionResult> LoadVehicleInventory()
        {
            await _vehicleLoadService.LoadVehicleInventoryAsync();
            return Ok(ResponseResult<string>.SuccessResult(null,"Load vehicle successfully."));
        }
        [HttpPost("LoadAuctionSetting")]
        public async Task<IActionResult> LoadAuctionSetting()
        {
            await _auctionService.ImportAuctionSettingAsync();
            return Ok(ResponseResult<string>.SuccessResult(null,"Import completed successfully."));
        }
        [HttpPost("LoadAuctionVehicle")]
        public async Task<IActionResult> LoadAuctionVehicle()
        {
            await _auctionLoadService.LoadAuctionVehiclesAsync();
            return Ok(ResponseResult<string>.SuccessResult(null,"Import completed successfully."));
        }
    }
}
