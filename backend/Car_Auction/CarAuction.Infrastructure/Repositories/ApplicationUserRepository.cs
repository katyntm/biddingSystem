using CarAuction.Domain.Entities;
using CarAuction.Domain.Interfaces;
using CarAuction.Infrastructure.Persistence;

namespace CarAuction.Infrastructure.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly CarAuctionDbContext _context;
        
        public ApplicationUserRepository(CarAuctionDbContext context)
        {
            _context = context;
        }
        
        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }
        
        public async Task<bool> UpdateUserBalanceAsync(string userId, decimal newBalance)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;
                
            user.Balance = newBalance;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}