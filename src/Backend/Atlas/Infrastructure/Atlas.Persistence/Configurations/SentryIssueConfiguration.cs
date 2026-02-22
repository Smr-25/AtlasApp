using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class SentryIssueConfiguration : IEntityTypeConfiguration<SentryIssue>
{
    public void Configure(EntityTypeBuilder<SentryIssue> builder)
    {
        builder.ToTable("SentryIssues", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.ExternalId).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Culprit).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Level).IsRequired();
        builder.Property(e => e.FileName).IsRequired().HasMaxLength(500);
        builder.Property(e => e.StackTrace).HasColumnType("text");
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.IntegrationId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

