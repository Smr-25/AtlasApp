using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class SquadRadarEntryConfiguration : IEntityTypeConfiguration<SquadRadarEntry>
{
    public void Configure(EntityTypeBuilder<SquadRadarEntry> builder)
    {
        builder.ToTable("SquadRadarEntries", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.OnlineStatus).IsRequired();
        builder.Property(e => e.CurrentToolIcon).HasMaxLength(50);
        builder.Property(e => e.CurrentFocus).HasMaxLength(200);
        builder.Property(e => e.ActiveIntegrationsJson).HasColumnType("jsonb");
        builder.HasIndex(e => new { e.TeamId, e.UserId }).IsUnique();
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

