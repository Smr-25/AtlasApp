using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class SonarQubeReportConfiguration : IEntityTypeConfiguration<SonarQubeReport>
{
    public void Configure(EntityTypeBuilder<SonarQubeReport> builder)
    {
        builder.ToTable("SonarQubeReports", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.ProjectKey).IsRequired().HasMaxLength(200);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.IntegrationId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

