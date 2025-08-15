using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    internal class BidHistoryConfiguration: IEntityTypeConfiguration<BidHistory>
    {
        public void Configure(EntityTypeBuilder<BidHistory> builder)
        {

            builder.HasKey(e => e.Id);

            builder.Property(e => e.BidAmount)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

            builder.Property(e => e.BidTime)
                      .HasDefaultValueSql("GETDATE()");

            builder.HasOne(e => e.AuctionVehicle)
                      .WithMany()
                      .HasForeignKey(e => e.AuctionVehicleId)
                      .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.BidderUser)
                      .WithMany()
                      .HasForeignKey(e => e.BidderUserId)
                      .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
