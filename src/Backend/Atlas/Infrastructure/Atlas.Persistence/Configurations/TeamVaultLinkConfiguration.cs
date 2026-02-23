using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class TeamVaultLinkConfiguration : IEntityTypeConfiguration<TeamVaultLink>
{
    public void Configure(EntityTypeBuilder<TeamVaultLink> builder)
    {
        builder.ToTable("TeamVaultLinks", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Label)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Url)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.Icon)
            .HasMaxLength(100);

        builder.HasIndex(e => e.TeamId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

