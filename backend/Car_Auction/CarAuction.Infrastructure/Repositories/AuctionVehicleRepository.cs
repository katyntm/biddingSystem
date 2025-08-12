using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Repositories.@base;

namespace CarAuction.Infrastructure.Repositories
{
    public class AuctionVehicleRepository : GenericRepository<AuctionVehicle>, IAuctionVehicleRepository
    {
        public AuctionVehicleRepository(CarAuctionDbContext context) : base(context)
        {
        }
    }
}
