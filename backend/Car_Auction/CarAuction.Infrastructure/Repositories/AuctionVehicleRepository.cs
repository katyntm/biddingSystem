using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Repositories.@base;
using Microsoft.EntityFrameworkCore;

namespace CarAuction.Infrastructure.Repositories
{
    public class AuctionVehicleRepository : GenericRepository<AuctionVehicle>, IAuctionVehicleRepository
    {

        public AuctionVehicleRepository(CarAuctionDbContext context) : base(context)
        {

        }

        public async Task<IEnumerable<AuctionVehicle>> GetWonAuctionsByUserAsync(string userId)
        {
            return await _context.Set<AuctionVehicle>()
                .Where(a => a.WinnerUserId == userId && a.IsSold)
                .Include(a => a.Vehicle)
                .ToListAsync();
        }
    }
}
