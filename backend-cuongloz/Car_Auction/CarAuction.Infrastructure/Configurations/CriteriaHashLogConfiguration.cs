using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    internal class CriteriaHashLogConfiguration : IEntityTypeConfiguration<CriteriaHashLog>
    {
        public void Configure(EntityTypeBuilder<CriteriaHashLog> builder)
        {
            builder.HasOne<Criteria>()
                .WithOne()
                .HasForeignKey<CriteriaHashLog>(h => h.CriteriaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
