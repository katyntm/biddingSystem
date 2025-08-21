using CarAuction.Application.Common;
using CarAuction.Infrastructure.Services.CronJobService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarAuction.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LoadingDataController : ControllerBase
    {
        private readonly ImportAuctionSetting _auctionService;
        private readonly LoadAuctionVehicle _auctionLoadService;

        public LoadingDataController(ImportAuctionSetting importService, LoadAuctionVehicle auctionLoadService)
        {
            _auctionService = importService;
            _auctionLoadService = auctionLoadService;
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
