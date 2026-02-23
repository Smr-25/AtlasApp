using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class SecurityScanResultConfiguration : IEntityTypeConfiguration<SecurityScanResult>
{
    public void Configure(EntityTypeBuilder<SecurityScanResult> builder)
    {
        builder.ToTable("SecurityScanResults", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.ScanType).IsRequired().HasMaxLength(100);
        builder.Property(e => e.ResultJson).IsRequired().HasColumnType("text");
        builder.HasIndex(e => e.UserId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

