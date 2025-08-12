using CarAuction.Domain.Interfaces.UnitOfWork;
using CarAuction.Infrastructure.Persistence;

namespace CarAuction.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CarAuctionDbContext _context;

        public UnitOfWork(CarAuctionDbContext context)
        {
            _context = context;
        }
        public async Task SaveChangeAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
