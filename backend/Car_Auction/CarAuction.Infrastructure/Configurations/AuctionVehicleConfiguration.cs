using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    internal class AuctionVehicleConfiguration: IEntityTypeConfiguration<AuctionVehicle>
    {
        public void Configure(EntityTypeBuilder<AuctionVehicle> builder)
        {

            builder.HasKey(e => e.Id);

            builder.Property(e => e.CurrentPrice)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");

            builder.Property(e => e.BuyItNowPrice)
                  .HasColumnType("decimal(18,2)");

            builder.Property(e => e.IsSold)
                  .HasDefaultValue(false);

            builder.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("GETDATE()");

            builder.Property(e => e.StartTime)
                  .IsRequired();
            //Concurrency
            builder.Property(e => e.EndTime)
                  .IsRequired();
            builder.Property(e => e.RowVersion).IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
            // Foreign Keys
            builder.HasOne(e => e.Tactic)
                  .WithMany()
                  .HasForeignKey(e => e.TacticId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Step)
                  .WithMany()
                  .HasForeignKey(e => e.StepId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.WinnerUser)
                  .WithMany()
                  .HasForeignKey(e => e.WinnerUserId)
                  .OnDelete(DeleteBehavior.SetNull);
          //      builder.HasOne<Vehicle>()
          //.WithMany()
          //.HasForeignKey("VehicleId");
        }
    }
}
