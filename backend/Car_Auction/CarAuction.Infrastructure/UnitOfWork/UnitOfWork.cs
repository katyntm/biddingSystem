using CarAuction.Domain.Interfaces.Repositories;
using CarAuction.Domain.Interfaces.UnitOfWork;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Repositories;

namespace CarAuction.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CarAuctionDbContext _context;
        private IVehicleRepository _vehicleRepository;
        private IVehicleImageRepository _vehicleImageRepository;

        public UnitOfWork(CarAuctionDbContext context)
        {
            _context = context;
        }

        public IVehicleRepository Vehicles => _vehicleRepository ??= new VehicleRepository(_context);

        public IVehicleImageRepository VehicleImages => _vehicleImageRepository ??= new VehicleImageRepository(_context);

        public async Task SaveChangeAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
