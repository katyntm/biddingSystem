using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Repositories.@base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarAuction.Infrastructure.Repositories
{
    public class BuyNowHistoryRepository : GenericRepository<BuyNowHistory>, IBuyNowHistoryRepository
    {
        private readonly CarAuctionDbContext _context;
        
        public BuyNowHistoryRepository(CarAuctionDbContext context) : base(context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<BuyNowHistory>> GetBuyHistoryForAuctionAsync(Guid auctionVehicleId)
        {
            return await _context.Set<BuyNowHistory>()
                .Where(b => b.AuctionVehicleId == auctionVehicleId)
                .OrderByDescending(b => b.BuyTime)
                .Include(b => b.BuyerUser)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<BuyNowHistory>> GetBuyHistoryByUserAsync(string userId)
        {
            return await _context.Set<BuyNowHistory>()
                .Where(b => b.BuyerUserId == userId)
                .OrderByDescending(b => b.BuyTime)
                .Include(b => b.AuctionVehicle)
                .ThenInclude(av => av.Vehicle)
                .ToListAsync();
        }
    }
}