using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class SquadArenaEntryConfiguration : IEntityTypeConfiguration<SquadArenaEntry>
{
    public void Configure(EntityTypeBuilder<SquadArenaEntry> builder)
    {
        builder.ToTable("SquadArenaEntries", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Points).IsRequired();
        builder.Property(e => e.BadgeType).IsRequired();
        builder.Property(e => e.SprintId).HasMaxLength(100);
        builder.Property(e => e.MetadataJson).HasColumnType("text");
        builder.HasIndex(e => new { e.TeamId, e.UserId });
        builder.HasIndex(e => new { e.TeamId, e.BadgeType });
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

