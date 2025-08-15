using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    internal class StepConfiguration : IEntityTypeConfiguration<Step>
    {
        public void Configure(EntityTypeBuilder<Step> builder)
        {

            builder.HasKey(e => e.Id);

            builder.Property(e => e.StepNumber)
                      .IsRequired();

            builder.HasOne(e => e.Tactic)
                        .WithMany(t => t.Steps)
                        .HasForeignKey(e => e.TacticId)
                        .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.SaleChannel)
                .WithMany()
                .HasForeignKey(e => e.SaleChannelId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
