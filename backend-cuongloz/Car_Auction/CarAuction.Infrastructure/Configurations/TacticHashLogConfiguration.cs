using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    internal class TacticHashLogConfiguration : IEntityTypeConfiguration<TacticHashLog>
    {
        public void Configure(EntityTypeBuilder<TacticHashLog> builder)
        {
            builder.HasOne<Tactic>()
                .WithOne()
                .HasForeignKey<TacticHashLog>(h => h.TacticId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
