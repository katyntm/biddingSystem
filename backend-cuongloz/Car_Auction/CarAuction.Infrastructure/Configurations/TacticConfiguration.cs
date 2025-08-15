using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    internal class TacticConfiguration : IEntityTypeConfiguration<Tactic>
    {
        public void Configure(EntityTypeBuilder<Tactic> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);

            builder.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");
        }
    }
}
