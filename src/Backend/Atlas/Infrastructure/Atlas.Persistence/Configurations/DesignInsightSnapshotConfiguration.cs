using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class DesignInsightSnapshotConfiguration : IEntityTypeConfiguration<DesignInsightSnapshot>
{
    public void Configure(EntityTypeBuilder<DesignInsightSnapshot> builder)
    {
        builder.ToTable("DesignInsightSnapshots", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.MetricKey).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Unit).HasMaxLength(50);
        builder.Property(e => e.MetadataJson).HasColumnType("text");
        builder.Property(e => e.Type).IsRequired();
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.Type, e.RecordedAt });
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

