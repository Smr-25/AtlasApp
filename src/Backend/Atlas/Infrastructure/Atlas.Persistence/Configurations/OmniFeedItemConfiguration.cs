using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class OmniFeedItemConfiguration : IEntityTypeConfiguration<OmniFeedItem>
{
    public void Configure(EntityTypeBuilder<OmniFeedItem> builder)
    {
        builder.ToTable("OmniFeedItems", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Source).IsRequired();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Body).HasMaxLength(4000);
        builder.Property(e => e.MetadataJson).HasColumnType("text");
        builder.Property(e => e.Emoji).HasMaxLength(10);
        builder.HasIndex(e => new { e.TeamId, e.Timestamp });
        builder.HasIndex(e => e.Source);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

