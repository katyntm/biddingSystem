using CarAuction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarAuction.Infrastructure.Configurations
{
    internal class CriteriaConfiguration : IEntityTypeConfiguration<Criteria>
    {
        public void Configure(EntityTypeBuilder<Criteria> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FieldName)
                      .IsRequired()
                      .HasMaxLength(100);

            builder.Property(e => e.Operator)
                      .IsRequired()
                      .HasMaxLength(100);

            builder.Property(e => e.Value)
                      .IsRequired()
                      .HasMaxLength(255);

            builder.HasOne(e => e.Tactic)
                      .WithMany(t => t.Criterias)
                      .HasForeignKey(e => e.TacticId)
                      .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
