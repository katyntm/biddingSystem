using CarAuction.Domain.Entities;

namespace CarAuction.Domain.Interfaces
{
    public interface IAuctionVehicleRepository : IGenericRepository<AuctionVehicle>
    {
        Task<IEnumerable<AuctionVehicle>> GetWonAuctionsByUserAsync(string userId);
    }
}
