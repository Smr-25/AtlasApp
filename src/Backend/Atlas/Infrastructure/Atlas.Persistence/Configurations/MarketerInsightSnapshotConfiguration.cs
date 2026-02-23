using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class MarketerInsightSnapshotConfiguration : IEntityTypeConfiguration<MarketerInsightSnapshot>
{
    public void Configure(EntityTypeBuilder<MarketerInsightSnapshot> builder)
    {
        builder.ToTable("MarketerInsightSnapshots", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.MetricKey).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Unit).HasMaxLength(50);
        builder.Property(e => e.MetadataJson).HasColumnType("text");
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.Type, e.RecordedAt });
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

