using CarAuction.Application.Common;
using CarAuction.Domain.Interfaces.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarAuction.API.Controllers
{
  [Route("api/reports")]
  [ApiController]
  [Authorize]
  public class ReportsController : ControllerBase
  {
    private readonly IUnitOfWork _unitOfWork;

    public ReportsController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    [HttpGet("bids")]
    public async Task<IActionResult> GetUserBids()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      // Use the new repository method that includes related entities
      var bids = await _unitOfWork.BidHistories.GetBidHistoryByUserAsync(userId);

      var result = bids
          .GroupBy(b => b.AuctionVehicleId)
          .Select(g =>
          {
            var auction = g.First().AuctionVehicle;
            // Skip if auction or vehicle is null
            if (auction == null || auction.Vehicle == null)
              return null;

            var vehicle = auction.Vehicle;
            var highestBid = g.OrderByDescending(b => b.BidAmount).First();
            var currentHighestBid = auction.CurrentPrice;
            var now = DateTime.Now;

            string status = "Unknown";
            if (auction.EndTime > now)
            {
              status = auction.WinnerUserId == userId ? "Leading" : "Outbid";
            }
            else
            {
              status = auction.WinnerUserId == userId ? "Ended - Won" : "Ended - Lost";
            }

            return new
            {
              Description = $"{vehicle.ModelYear} {vehicle.Make} {vehicle.ModelType}",
              VIN = vehicle.VIN,
              MyHighestBid = highestBid.BidAmount,
              Price = currentHighestBid,
              Date = highestBid.BidTime,
              SalesChannel = auction.Step?.SaleChannel?.Name ?? "Online Auction",
              Status = status
            };
          })
          .Where(r => r != null) // Filter out any null results
          .ToList();

      return Ok(ResponseResult<object>.SuccessResult(result));
    }

    [HttpGet("purchases")]
    public async Task<IActionResult> GetUserPurchases()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

      // Get Buy Now purchases
      var buyNowPurchases = await _unitOfWork.BuyNowHistories.GetBuyHistoryByUserAsync(userId);
      buyNowPurchases = buyNowPurchases.Where(p => p.BuyerUserId == userId);

      var buyNowResults = buyNowPurchases
          .Where(p => p.AuctionVehicle != null && p.AuctionVehicle.Vehicle != null)
          .Select(p => new
          {
            Description = $"{p.AuctionVehicle.Vehicle.ModelYear} {p.AuctionVehicle.Vehicle.Make} {p.AuctionVehicle.Vehicle.ModelType}",
            VIN = p.AuctionVehicle.Vehicle.VIN,
            PurchaseAmount = p.BuyAmount,
            PurchaseDate = p.BuyTime,
            PurchaseType = "Buy Now"
          }).ToList();

      // Get won auctions
      var auctionVehicles = await _unitOfWork.AuctionVehicles.GetAllAsync();
      var wonAuctions = auctionVehicles
          .Where(a => a.WinnerUserId == userId && a.IsSold && a.Vehicle != null);

      var auctionResults = wonAuctions.Select(a => new
      {
        Description = $"{a.Vehicle.ModelYear} {a.Vehicle.Make} {a.Vehicle.ModelType}",
        VIN = a.Vehicle.VIN,
        PurchaseAmount = a.CurrentPrice,
        PurchaseDate = a.EndTime,
        PurchaseType = "Auction"
      }).ToList();

      var combinedResults = buyNowResults.Concat(auctionResults).OrderByDescending(r => r.PurchaseDate);

      return Ok(ResponseResult<object>.SuccessResult(combinedResults));
    }
  }
}