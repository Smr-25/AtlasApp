using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class SecOpsAlertConfiguration : IEntityTypeConfiguration<SecOpsAlert>
{
    public void Configure(EntityTypeBuilder<SecOpsAlert> builder)
    {
        builder.ToTable("SecOpsAlerts", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Message).IsRequired().HasMaxLength(2000);
        builder.Property(e => e.ActionPayload).HasColumnType("text");
        builder.Property(e => e.Type).IsRequired();
        builder.Property(e => e.Severity).IsRequired();
        builder.HasIndex(e => e.UserId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

