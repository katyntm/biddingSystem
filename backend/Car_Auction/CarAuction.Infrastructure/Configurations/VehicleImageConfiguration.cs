using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    public class VehicleImageConfiguration : IEntityTypeConfiguration<VehicleImage>
    {
        public void Configure(EntityTypeBuilder<VehicleImage> builder)
        {
            builder.HasKey(i => i.Id);
            
            builder.Property(i => i.Url)
                .IsRequired()
                .HasMaxLength(500);
        }
    }
}