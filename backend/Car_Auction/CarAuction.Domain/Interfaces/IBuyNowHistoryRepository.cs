
using CarAuction.Domain.Entities;

namespace CarAuction.Domain.Interfaces
{
    public interface IBuyNowHistoryRepository : IGenericRepository<BuyNowHistory>
    {
        Task<IEnumerable<BuyNowHistory>> GetBuyHistoryForAuctionAsync(Guid auctionVehicleId);
        Task<IEnumerable<BuyNowHistory>> GetBuyHistoryByUserAsync(string userId);
    }
}