using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class QuickCaptureConfiguration : IEntityTypeConfiguration<QuickCapture>
{
    public void Configure(EntityTypeBuilder<QuickCapture> builder)
    {
        builder.ToTable("QuickCaptures", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(e => e.Title)
            .HasMaxLength(500);

        builder.Property(e => e.Url)
            .HasMaxLength(2000);

        builder.Property(e => e.Source)
            .IsRequired();

        builder.Property(e => e.ExternalId)
            .HasMaxLength(500);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.Source });
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

