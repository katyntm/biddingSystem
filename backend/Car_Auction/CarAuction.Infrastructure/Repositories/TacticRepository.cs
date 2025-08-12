using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Repositories.@base;


namespace CarAuction.Infrastructure.Repositories
{
    public class TacticRepository : GenericRepository<Tactic>, ITacticRepository
    {
        public TacticRepository(CarAuctionDbContext context) : base(context)
        {
        }
    }
}
