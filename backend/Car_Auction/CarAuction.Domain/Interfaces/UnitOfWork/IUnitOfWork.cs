namespace CarAuction.Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IVehicleRepository Vehicles { get; }
        IVehicleImageRepository VehicleImages { get; }
        IApplicationUserRepository ApplicationUsers { get; }
        IAuctionVehicleRepository AuctionVehicles { get; }
        IBuyNowHistoryRepository BuyNowHistories { get; }
        IBidHistoryRepository BidHistories { get; }
        
        Task<int> SaveChangesAsync();

        // Add transaction support
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}