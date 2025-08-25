using CarAuction.Application.Hubs;
using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces.UnitOfWork;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarAuction.Infrastructure.Services
{
    public class AuctionEndService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuctionEndService> _logger;
        private readonly IHubContext<AuctionHub> _hubContext;

        public AuctionEndService(
            IUnitOfWork unitOfWork,
            ILogger<AuctionEndService> logger,
            IHubContext<AuctionHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task ProcessEndedAuctionsAsync()
        {
            try
            {
                // Get all auctions that have ended but not been sold
                var currentTime = DateTime.Now;
                var endedAuctions = await _unitOfWork.AuctionVehicles.GetAllAsync();
                endedAuctions = endedAuctions.Where(a => 
                    a.EndTime <= currentTime && 
                    !a.IsSold).ToList();

                _logger.LogInformation($"Processing {endedAuctions.Count()} ended auctions");

                foreach (var auction in endedAuctions)
                {
                    await ProcessAuctionResultAsync(auction);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ended auctions");
            }
        }

        private async Task ProcessAuctionResultAsync(AuctionVehicle auction)
        {
            try
            {
                // Start transaction for this auction
                await _unitOfWork.BeginTransactionAsync();

                // Get the highest bid for this auction
                var highestBid = await _unitOfWork.BidHistories.GetLastBidForAuctionAsync(auction.Id);

                if (highestBid != null)
                {
                    // There is a winning bid
                    auction.IsSold = true;
                    auction.WinnerUserId = highestBid.BidderUserId;
                    auction.CurrentPrice = highestBid.BidAmount;
                    
                    // Update winner's balance
                    var winner = await _unitOfWork.ApplicationUsers.GetByIdAsync(highestBid.BidderUserId);
                    if (winner != null)
                    {
                        decimal newBalance = winner.Balance - highestBid.BidAmount;
                        await _unitOfWork.ApplicationUsers.UpdateUserBalanceAsync(highestBid.BidderUserId, newBalance);
                        _logger.LogInformation($"Updated balance for user {highestBid.BidderUserId}: {winner.Balance} -> {newBalance}");
                    }
                }
                
                _unitOfWork.AuctionVehicles.Update(auction);

                // Save changes before committing transaction
                await _unitOfWork.SaveChangesAsync();

                // Commit the transaction
                await _unitOfWork.CommitTransactionAsync();
                
                // Notify clients about auction end
                await NotifyAuctionEndAsync(auction);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error processing auction {auction.Id}");
            }
        }

        private async Task NotifyAuctionEndAsync(AuctionVehicle auction)
        {
            try
            {
                await _hubContext.Clients.Group($"vehicle:{auction.Id}").SendAsync("AuctionEnded", new
                {
                    AuctionVehicleId = auction.Id,
                    FinalPrice = auction.CurrentPrice,
                    WinnerUserId = auction.WinnerUserId,
                    IsSold = auction.IsSold
                });

                // If there's a winner, notify them specifically
                if (auction.WinnerUserId != null)
                {
                    await _hubContext.Clients.Group($"user:{auction.WinnerUserId}").SendAsync("AuctionWon", new
                    {
                        AuctionVehicleId = auction.Id,
                        FinalPrice = auction.CurrentPrice,
                        VehicleVIN = auction.Vehicle.VIN,
                        VehicleMake = auction.Vehicle.Make,
                        VehicleModel = auction.Vehicle.ModelType
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error notifying clients about auction end for auction {auction.Id}");
            }
        }
    }
}