using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "atlas");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.Action).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(a => a.Description).IsRequired().HasMaxLength(1000);
        builder.Property(a => a.EntityName).HasMaxLength(100);
        builder.Property(a => a.IpAddress).HasMaxLength(50);
        builder.Property(a => a.UserAgent).HasMaxLength(500);
        builder.Property(a => a.MetadataJson).HasColumnType("jsonb");
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(a => a.UserId).HasDatabaseName("IX_AuditLogs_UserId");
        builder.HasIndex(a => new { a.UserId, a.Action }).HasDatabaseName("IX_AuditLogs_UserId_Action");
        builder.HasIndex(a => a.CreatedAt).HasDatabaseName("IX_AuditLogs_CreatedAt");

        builder.Ignore(a => a.DomainEvents);
    }
}

