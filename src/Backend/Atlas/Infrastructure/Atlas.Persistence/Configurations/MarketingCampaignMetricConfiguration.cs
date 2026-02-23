using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class MarketingCampaignMetricConfiguration : IEntityTypeConfiguration<MarketingCampaignMetric>
{
    public void Configure(EntityTypeBuilder<MarketingCampaignMetric> builder)
    {
        builder.ToTable("MarketingCampaignMetrics", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CampaignId).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Platform).IsRequired().HasMaxLength(100);
        builder.Property(e => e.MetadataJson).HasColumnType("text");
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.RecordedAt });
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

