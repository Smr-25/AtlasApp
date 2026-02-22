using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class DesignHandoffConfiguration : IEntityTypeConfiguration<DesignHandoff>
{
    public void Configure(EntityTypeBuilder<DesignHandoff> builder)
    {
        builder.ToTable("DesignHandoffs", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.DesignName).IsRequired().HasMaxLength(300);
        builder.Property(e => e.FigmaFileUrl).HasMaxLength(1000);
        builder.Property(e => e.ZeplinScreenUrl).HasMaxLength(1000);
        builder.Property(e => e.Status).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Notes).HasMaxLength(2000);
        builder.HasIndex(e => e.DesignerId);
        builder.HasIndex(e => e.DeveloperId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

