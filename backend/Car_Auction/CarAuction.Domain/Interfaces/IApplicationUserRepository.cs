using CarAuction.Domain.Entities;
using System.Threading.Tasks;

namespace CarAuction.Domain.Interfaces
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<bool> UpdateUserBalanceAsync(string userId, decimal newBalance);
    }
}