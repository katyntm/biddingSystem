using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    internal class SaleChannelConfiguration : IEntityTypeConfiguration<SaleChannel>
    {
        public void Configure(EntityTypeBuilder<SaleChannel> builder)
        {

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);

            builder.Property(e => e.PricePercentage)
                      .IsRequired()
                      .HasColumnType("decimal(5,2)");

            builder.Property(e => e.BuyItNowPercentage)
                      .HasColumnType("decimal(5,2)");
        }
    }
}
