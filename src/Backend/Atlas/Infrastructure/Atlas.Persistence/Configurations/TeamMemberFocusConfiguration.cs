using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class TeamMemberFocusConfiguration : IEntityTypeConfiguration<TeamMemberFocus>
{
    public void Configure(EntityTypeBuilder<TeamMemberFocus> builder)
    {
        builder.ToTable("TeamMemberFocuses", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.FocusDescription)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(e => e.TeamId);
        builder.HasIndex(e => new { e.TeamMemberId, e.IsActive });
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

