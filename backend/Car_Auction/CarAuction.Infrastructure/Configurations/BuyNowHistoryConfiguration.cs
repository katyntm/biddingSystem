using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    internal class BuyNowHistoryConfiguration: IEntityTypeConfiguration<BuyNowHistory>
    {
        public void Configure(EntityTypeBuilder<BuyNowHistory> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.BuyAmount)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

            builder.Property(e => e.BuyTime)
                      .HasDefaultValueSql("GETDATE()");

            builder.HasOne(e => e.AuctionVehicle)
                      .WithMany()
                      .HasForeignKey(e => e.AuctionVehicleId)
                      .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.BuyerUser)
                      .WithMany()
                      .HasForeignKey(e => e.BuyerUserId)
                      .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
