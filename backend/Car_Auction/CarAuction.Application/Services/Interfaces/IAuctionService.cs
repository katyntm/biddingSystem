using CarAuction.Application.DTOs;
using System.Threading.Tasks;

namespace CarAuction.Application.Interfaces.Services
{
    public interface IAuctionService
    {
        Task<AuctionActionResultDto> PlaceBidAsync(Guid auctionVehicleId, string userId, decimal bidAmount);
        Task<AuctionActionResultDto> BuyNowAsync(Guid auctionVehicleId, string userId);
    }
}