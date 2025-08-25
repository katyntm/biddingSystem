using CarAuction.Domain.Interfaces;
using CarAuction.Domain.Interfaces.UnitOfWork;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace CarAuction.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CarAuctionDbContext _context;
        private IDbContextTransaction _transaction;
        
        private IVehicleRepository _vehicleRepository;
        private IVehicleImageRepository _vehicleImageRepository;
        private IApplicationUserRepository _applicationUserRepository;
        private IAuctionVehicleRepository _auctionVehicleRepository;
        private IBuyNowHistoryRepository _buyNowHistoryRepository;
        private IBidHistoryRepository _bidHistoryRepository;

        public UnitOfWork(CarAuctionDbContext context)
        {
            _context = context;
        }

        public IVehicleRepository Vehicles => _vehicleRepository ??= new VehicleRepository(_context);

        public IVehicleImageRepository VehicleImages => _vehicleImageRepository ??= new VehicleImageRepository(_context);

        public IApplicationUserRepository ApplicationUsers => _applicationUserRepository ??= new ApplicationUserRepository(_context);

        public IAuctionVehicleRepository AuctionVehicles => _auctionVehicleRepository ??= new AuctionVehicleRepository(_context);

        public IBuyNowHistoryRepository BuyNowHistories => _buyNowHistoryRepository ??= new BuyNowHistoryRepository(_context);

        public IBidHistoryRepository BidHistories => _bidHistoryRepository ??= new BidHistoryRepository(_context);

        // public void Dispose() => _context.Dispose();

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
