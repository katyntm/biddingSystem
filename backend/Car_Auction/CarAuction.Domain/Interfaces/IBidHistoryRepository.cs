
using CarAuction.Domain.Entities;

namespace CarAuction.Domain.Interfaces
{
  public interface IBidHistoryRepository : IGenericRepository<BidHistory>
  {
    Task<BidHistory> GetLastBidForAuctionAsync(Guid auctionVehicleId);
    Task<IEnumerable<BidHistory>> GetBidHistoryForAuctionAsync(Guid auctionVehicleId);
    Task<IEnumerable<BidHistory>> GetBidHistoryByUserAsync(string userId);
  }
}