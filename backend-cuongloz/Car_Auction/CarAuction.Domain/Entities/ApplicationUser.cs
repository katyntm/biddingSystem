using CarAuction.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace CarAuction.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public  CreditStatus CreditStatus {  get; set; }
        public decimal Balance { get; set; }
    }
}
