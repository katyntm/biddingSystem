using CarAuction.Application.DTOs;
using CarAuction.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarAuction.API.Controllers
{
  [Route("api")]
  [ApiController]
  // [Authorize(Roles = "Dealer")]
  [Authorize]
  public class AuctionActionsController : ControllerBase
  {
    private readonly IAuctionService _auctionService;

    public AuctionActionsController(IAuctionService auctionService)
    {
      _auctionService = auctionService;
    }

    [HttpPost("bid")]
    public async Task<IActionResult> PlaceBid([FromBody] BidRequestDto bidRequest)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      var result = await _auctionService.PlaceBidAsync(bidRequest.AuctionVehicleId, userId, bidRequest.BidAmount);

      if (!result.Success)
        return BadRequest(result.Message);

      return Ok(result);
    }

    [HttpPost("buy")]
    public async Task<IActionResult> BuyNow([FromBody] BuyNowRequestDto buyRequest)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      var result = await _auctionService.BuyNowAsync(buyRequest.AuctionVehicleId, userId);

      if (!result.Success)
        return BadRequest(result.Message);

      return Ok(result);
    }
  }
}