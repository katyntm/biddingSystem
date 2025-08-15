using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    internal class StepHashLogConfiguration : IEntityTypeConfiguration<StepHashLog>
    {
        public void Configure(EntityTypeBuilder<StepHashLog> builder)
        {
            builder.HasOne<Step>()
                .WithOne()
                .HasForeignKey<StepHashLog>(h => h.StepId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
