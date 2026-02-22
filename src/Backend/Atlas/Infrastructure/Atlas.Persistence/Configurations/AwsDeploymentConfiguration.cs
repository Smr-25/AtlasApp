using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class AwsDeploymentConfiguration : IEntityTypeConfiguration<AwsDeployment>
{
    public void Configure(EntityTypeBuilder<AwsDeployment> builder)
    {
        builder.ToTable("AwsDeployments", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.ServiceName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Environment).IsRequired().HasMaxLength(50);
        builder.Property(e => e.CommitSha).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.LogUrl).HasMaxLength(1000);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.IntegrationId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

