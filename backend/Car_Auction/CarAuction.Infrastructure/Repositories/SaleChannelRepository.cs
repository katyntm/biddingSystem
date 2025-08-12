using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Repositories.@base;

namespace CarAuction.Infrastructure.Repositories
{
    public class SaleChannelRepository : GenericRepository<SaleChannel>, ISaleChannelRepository
    {
        public SaleChannelRepository(CarAuctionDbContext context) : base(context)
        {
        }
    }
}
