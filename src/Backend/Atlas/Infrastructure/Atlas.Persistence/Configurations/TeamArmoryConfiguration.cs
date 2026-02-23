using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class TeamArmoryConfiguration : IEntityTypeConfiguration<TeamArmory>
{
    public void Configure(EntityTypeBuilder<TeamArmory> builder)
    {
        builder.ToTable("TeamArmories", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.StagingServerUrl)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.TestAccountEmail)
            .HasMaxLength(200);

        builder.Property(e => e.TestAccountPassword)
            .HasMaxLength(200);

        builder.Property(e => e.ProductionVersion)
            .HasMaxLength(50);

        builder.Property(e => e.StagingVersion)
            .HasMaxLength(50);

        builder.HasIndex(e => e.TeamId).IsUnique();
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

