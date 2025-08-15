using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Repositories.@base;


namespace CarAuction.Infrastructure.Repositories
{
    public class CriteriaRepository : GenericRepository<Criteria>, ICriteriaRepository
    {
        public CriteriaRepository(CarAuctionDbContext context) : base(context)
        {
        }
    }
}
