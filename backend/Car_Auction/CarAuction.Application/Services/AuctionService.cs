using CarAuction.Application.DTOs;
using CarAuction.Application.Hubs;
using CarAuction.Application.Interfaces.Services;
using CarAuction.Domain.Entities;
using CarAuction.Domain.Enums;
using CarAuction.Domain.Interfaces;
using CarAuction.Domain.Interfaces.UnitOfWork;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace CarAuction.Application.Services
{
  public class AuctionService : IAuctionService
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuctionService> _logger;
    private readonly IHubContext<AuctionHub> _hubContext;
    private readonly decimal _bidIncrement = 100; // Default increment, could be configurable

    public AuctionService(
        IUnitOfWork unitOfWork,
        ILogger<AuctionService> logger,
        IHubContext<AuctionHub> hubContext)
    {
      _unitOfWork = unitOfWork;
      _logger = logger;
      _hubContext = hubContext;
    }

    public async Task<AuctionActionResultDto> PlaceBidAsync(Guid auctionVehicleId, string userId, decimal bidAmount)
    {
      try
      {
        var auctionVehicleRepo = _unitOfWork.AuctionVehicles;
        var userRepo = _unitOfWork.ApplicationUsers;

        // Get auction vehicle with concurrency control
        var auctionVehicle = await auctionVehicleRepo.GetByIdAsync(auctionVehicleId);
        if (auctionVehicle == null)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Auction vehicle not found"
          };
        }

        // Check if auction is active
        if (DateTime.Now < auctionVehicle.StartTime || DateTime.Now > auctionVehicle.EndTime)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Auction is not active"
          };
        }

        // Check if vehicle is already sold
        if (auctionVehicle.IsSold)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Vehicle is already sold"
          };
        }

        // Validate bid amount (must be greater than current price + increment)
        if (bidAmount <= auctionVehicle.CurrentPrice + _bidIncrement)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = $"Bid amount must be at least {auctionVehicle.CurrentPrice + _bidIncrement}"
          };
        }

        // Get bidder
        var bidder = await userRepo.GetByIdAsync(userId);
        if (bidder == null)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Bidder not found"
          };
        }

        // Check bidder's credit status and balance
        if (bidder.CreditStatus != CreditStatus.Active)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Your credit status is not approved for bidding"
          };
        }

        if (bidder.Balance < bidAmount)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Insufficient balance for this bid"
          };
        }

        // Get previous high bidder (if any) for notification
        string previousHighBidderId = null;
        var lastBid = await _unitOfWork.BidHistories.GetLastBidForAuctionAsync(auctionVehicleId);

        if (lastBid != null)
        {
          previousHighBidderId = lastBid.BidderUserId;
        }

        // Create bid history record
        var bidHistory = new BidHistory
        {
          AuctionVehicleId = auctionVehicleId,
          BidderUserId = userId,
          BidAmount = bidAmount,
          BidTime = DateTime.Now
        };

        // Begin transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
          // Add bid history
          await _unitOfWork.BidHistories.AddAsync(bidHistory);

          // Update auction vehicle
          auctionVehicle.CurrentPrice = bidAmount;

          // If bid price exceeds buy it now price, disable buy it now option
          bool canBuyNow = auctionVehicle.CurrentPrice < auctionVehicle.BuyItNowPrice;

          // Update the auction vehicle
          auctionVehicleRepo.Update(auctionVehicle);

          // Commit transaction
          await _unitOfWork.CommitTransactionAsync();

          // Broadcast update to all clients watching this vehicle
          await _hubContext.Clients.Group($"vehicle:{auctionVehicleId}").SendAsync("BidPlaced", new
          {
            AuctionVehicleId = Guid.Parse(auctionVehicleId.ToString()),
            CurrentPrice = bidAmount,
            BidderUserId = userId,
            BidTime = DateTime.Now,
            IsSold = false,
            CanBuyNow = canBuyNow
          });

          // Notify previous high bidder that they've been outbid
          if (previousHighBidderId != null && previousHighBidderId != userId)
          {
            await _hubContext.Clients.Group($"user:{previousHighBidderId}").SendAsync("OutBid", new
            {
              AuctionVehicleId = auctionVehicleId,
              NewPrice = bidAmount
            });
          }

          return new AuctionActionResultDto
          {
            Success = true,
            Message = "Bid placed successfully",
            AuctionVehicleId = auctionVehicleId,
            CurrentPrice = bidAmount,
            IsSold = false,
            CanBid = true,
            CanBuyNow = canBuyNow
          };
        }
        catch (DbUpdateConcurrencyException)
        {
          await _unitOfWork.RollbackTransactionAsync();
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Another bid was placed. Please refresh and try again."
          };
        }
        catch (Exception ex)
        {
          await _unitOfWork.RollbackTransactionAsync();
          _logger.LogError(ex, "Error placing bid for auction vehicle {AuctionVehicleId}", auctionVehicleId);
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "An error occurred while processing your bid"
          };
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in PlaceBidAsync for auction vehicle {AuctionVehicleId}", auctionVehicleId);
        return new AuctionActionResultDto
        {
          Success = false,
          Message = "An unexpected error occurred"
        };
      }
    }

    public async Task<AuctionActionResultDto> BuyNowAsync(Guid auctionVehicleId, string userId)
    {
      try
      {
        var auctionVehicleRepo = _unitOfWork.AuctionVehicles;
        var userRepo = _unitOfWork.ApplicationUsers;

        // Get auction vehicle with concurrency control
        var auctionVehicle = await auctionVehicleRepo.GetByIdAsync(auctionVehicleId);
        if (auctionVehicle == null)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Auction vehicle not found"
          };
        }

        // Check if auction is active
        if (DateTime.Now < auctionVehicle.StartTime || DateTime.Now > auctionVehicle.EndTime)
        {
          System.Console.WriteLine("Current Time: " + DateTime.Now, " Start Time: " + auctionVehicle.StartTime, " End Time: " + auctionVehicle.EndTime);
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Auction is not active"
          };
        }

        // Check if vehicle is already sold
        if (auctionVehicle.IsSold)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Vehicle is already sold"
          };
        }

        // Check if buy it now is disabled (when current price exceeds buy it now price)
        if (auctionVehicle.CurrentPrice >= auctionVehicle.BuyItNowPrice)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Buy It Now is no longer available for this vehicle"
          };
        }

        // Get buyer
        var buyer = await userRepo.GetByIdAsync(userId);
        if (buyer == null)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Buyer not found"
          };
        }

        // Check buyer's credit status and balance
        if (buyer.CreditStatus != CreditStatus.Active)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Your credit status is not approved for purchasing"
          };
        }

        if (buyer.Balance < auctionVehicle.BuyItNowPrice)
        {
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Insufficient balance for this purchase"
          };
        }

        decimal newBalance = buyer.Balance - auctionVehicle.BuyItNowPrice;

        // Create buy now history record
        var buyNowHistory = new BuyNowHistory
        {
          AuctionVehicleId = auctionVehicleId,
          BuyerUserId = userId,
          BuyAmount = auctionVehicle.BuyItNowPrice,
          BuyTime = DateTime.Now
        };

        // Begin transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
          // Add buy now history
          await _unitOfWork.BuyNowHistories.AddAsync(buyNowHistory);

          // Update buyer's balance
          await userRepo.UpdateUserBalanceAsync(userId, newBalance);

          // Update auction vehicle
          auctionVehicle.IsSold = true;
          auctionVehicle.WinnerUserId = userId;
          auctionVehicle.CurrentPrice = auctionVehicle.BuyItNowPrice;

          // Update the auction vehicle
          auctionVehicleRepo.Update(auctionVehicle);

          // Commit transaction
          await _unitOfWork.CommitTransactionAsync();

          // Broadcast update to all clients watching this vehicle
          await _hubContext.Clients.Group($"vehicle:{auctionVehicleId}").SendAsync("VehicleSold", new
          {
            AuctionVehicleId = auctionVehicleId,
            FinalPrice = auctionVehicle.BuyItNowPrice,
            BuyerUserId = userId,
            BuyTime = DateTime.Now,
            IsSold = true
          });

          return new AuctionActionResultDto
          {
            Success = true,
            Message = "Vehicle purchased successfully",
            AuctionVehicleId = auctionVehicleId,
            CurrentPrice = auctionVehicle.BuyItNowPrice,
            IsSold = true,
            WinnerUserId = userId,
            CanBid = false,
            CanBuyNow = false
          };
        }
        catch (DbUpdateConcurrencyException)
        {
          await _unitOfWork.RollbackTransactionAsync();
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "Vehicle has been updated or purchased. Please refresh and try again."
          };
        }
        catch (Exception ex)
        {
          await _unitOfWork.RollbackTransactionAsync();
          _logger.LogError(ex, "Error completing purchase for auction vehicle {AuctionVehicleId}", auctionVehicleId);
          return new AuctionActionResultDto
          {
            Success = false,
            Message = "An error occurred while processing your purchase"
          };
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in BuyNowAsync for auction vehicle {AuctionVehicleId}", auctionVehicleId);
        return new AuctionActionResultDto
        {
          Success = false,
          Message = "An unexpected error occurred"
        };
      }
    }
  }
}