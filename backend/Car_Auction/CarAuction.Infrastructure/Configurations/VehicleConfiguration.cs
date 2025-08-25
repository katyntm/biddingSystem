using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.VIN)
                .IsRequired();

            builder.HasIndex(v => v.VIN)
                .IsUnique();

            builder.Property(v => v.Make)
                .IsRequired();

            builder.Property(v => v.ModelType)
                .IsRequired();

            builder.Property(v => v.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(v => v.Grade)
                .IsRequired();

            builder.Property(v => v.FuelType)
                .IsRequired();

            builder.Property(v => v.BodyStyle)
                .IsRequired();

            builder.Property(v => v.Color)
                .IsRequired();

            builder.Property(v => v.Transmission)
                .IsRequired();

            builder.Property(v => v.Location)
                .IsRequired();

            builder.Property(v => v.ModelYear)
                .IsRequired();

            builder.Property(v => v.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(v => v.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Navigation properties
            builder.HasMany(v => v.VehicleImages)
                .WithOne(i => i.Vehicle)
                .HasForeignKey(i => i.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}