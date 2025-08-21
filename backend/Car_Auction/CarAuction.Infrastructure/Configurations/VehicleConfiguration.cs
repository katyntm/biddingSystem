using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Persistence.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasKey(v => v.Id);
            
            builder.Property(v => v.VIN)
                .IsRequired()
                .HasMaxLength(17);
                
            builder.HasIndex(v => v.VIN)
                .IsUnique();
                
            builder.Property(v => v.Make)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(v => v.ModelType)
                .HasMaxLength(100);
                
            builder.Property(v => v.Price)
                .HasColumnType("decimal(18,2)");
                
            builder.Property(v => v.Grade)
                .HasColumnType("decimal(3,1)");
                
            // Navigation properties
            builder.HasMany(v => v.VehicleImages)
                .WithOne(i => i.Vehicle)
                .HasForeignKey(i => i.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}