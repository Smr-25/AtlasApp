using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class SharedResourceConfiguration : IEntityTypeConfiguration<SharedResource>
{
    public void Configure(EntityTypeBuilder<SharedResource> builder)
    {
        builder.ToTable("SharedResources", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.Url).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.Category).IsRequired();
        builder.HasIndex(e => e.TeamId);
        builder.HasIndex(e => new { e.TeamId, e.Category });
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

