using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces;
using CarAuction.Infrastructure.Persistence;
using CarAuction.Infrastructure.Repositories.@base;


namespace CarAuction.Infrastructure.Repositories
{
    public class StepRepository : GenericRepository<Step>, IStepRepository
    {
        public StepRepository(CarAuctionDbContext context) : base(context)
        {
        }
    }
}
