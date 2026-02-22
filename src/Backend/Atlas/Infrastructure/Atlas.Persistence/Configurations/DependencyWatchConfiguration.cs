using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class DependencyWatchConfiguration : IEntityTypeConfiguration<DependencyWatch>
{
    public void Configure(EntityTypeBuilder<DependencyWatch> builder)
    {
        builder.ToTable("DependencyWatches", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.PackageName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.CurrentVersion).IsRequired().HasMaxLength(50);
        builder.Property(e => e.LatestVersion).HasMaxLength(50);
        builder.Property(e => e.ProjectPath).IsRequired().HasMaxLength(500);
        builder.Property(e => e.VulnerabilityDetail).HasMaxLength(2000);
        builder.HasIndex(e => e.UserId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

