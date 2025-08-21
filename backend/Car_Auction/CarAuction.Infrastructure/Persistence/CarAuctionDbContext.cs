using CarAuction.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarAuction.Infrastructure.Persistence
{
    public class CarAuctionDbContext : IdentityDbContext<ApplicationUser>
    {
        public CarAuctionDbContext(DbContextOptions<CarAuctionDbContext> options) : base(options) { }

        public DbSet<AuctionVehicle> AuctionVehicles { get; set; }
        public DbSet<Tactic> Tactics { get; set; }
        public DbSet<Criteria> Criteria { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<SaleChannel> SaleChannels { get; set; }
        public DbSet<VehicleImage> VehicleImages { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(CarAuctionDbContext).Assembly);
        }
    }
}
