using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Repositories.@base;
using Microsoft.EntityFrameworkCore;

namespace CarAuction.Infrastructure.Repositories
{
  public class BidHistoryRepository : GenericRepository<BidHistory>, IBidHistoryRepository
  {
    private readonly CarAuctionDbContext _context;

    public BidHistoryRepository(CarAuctionDbContext context) : base(context)
    {
      _context = context;
    }

    public async Task<BidHistory> GetLastBidForAuctionAsync(Guid auctionVehicleId)
    {
      return await _context.Set<BidHistory>()
          .Where(b => b.AuctionVehicleId == auctionVehicleId)
          .OrderByDescending(b => b.BidTime)
          .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<BidHistory>> GetBidHistoryForAuctionAsync(Guid auctionVehicleId)
    {
      return await _context.Set<BidHistory>()
          .Where(b => b.AuctionVehicleId == auctionVehicleId)
          .OrderByDescending(b => b.BidTime)
          .Include(b => b.BidderUser)
          .ToListAsync();
    }

    public async Task<IEnumerable<BidHistory>> GetBidHistoryByUserAsync(string userId)
    {
      return await _context.Set<BidHistory>()
          .Where(b => b.BidderUserId == userId)
          .OrderByDescending(b => b.BidTime)
          .Include(b => b.AuctionVehicle)
            .ThenInclude(av => av.Vehicle)
          .Include(b => b.AuctionVehicle)
            .ThenInclude(av => av.Step)
              .ThenInclude(s => s.SaleChannel)
          .ToListAsync();
    }
  }
}