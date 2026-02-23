using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class TeamObjectiveConfiguration : IEntityTypeConfiguration<TeamObjective>
{
    public void Configure(EntityTypeBuilder<TeamObjective> builder)
    {
        builder.ToTable("TeamObjectives", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.HasIndex(e => e.TeamId);
        builder.HasIndex(e => new { e.TeamId, e.IsActive });
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

