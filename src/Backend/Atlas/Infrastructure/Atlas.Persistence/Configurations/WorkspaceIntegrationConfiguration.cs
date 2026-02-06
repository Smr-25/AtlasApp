using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class WorkspaceIntegrationConfiguration : IEntityTypeConfiguration<WorkspaceIntegration>
{
    public void Configure(EntityTypeBuilder<WorkspaceIntegration> builder)
    {
        builder.ToTable("WorkspaceIntegrations", "atlas");
        
        builder.HasKey(wi => wi.Id);
        
        builder.Property(wi => wi.Id)
            .ValueGeneratedNever();
        
        builder.Property(wi => wi.CreatedAt)
            .IsRequired();
        
        builder.Property(wi => wi.ModifiedAt);
        
        builder.Property(wi => wi.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(wi => wi.WorkspaceId)
            .IsRequired();
        
        builder.Property(wi => wi.IntegrationId)
            .IsRequired();
        
        builder.Property(wi => wi.SettingsJson)
            .HasColumnType("jsonb");
        
        builder.HasIndex(wi => wi.WorkspaceId)
            .HasDatabaseName("IX_WorkspaceIntegrations_WorkspaceId");
        
        builder.HasIndex(wi => wi.IntegrationId)
            .HasDatabaseName("IX_WorkspaceIntegrations_IntegrationId");
        
        builder.HasIndex(wi => new { wi.WorkspaceId, wi.IntegrationId })
            .HasDatabaseName("IX_WorkspaceIntegrations_WorkspaceId_IntegrationId")
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
        
        builder.HasOne(wi => wi.Integration)
            .WithMany()
            .HasForeignKey(wi => wi.IntegrationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasQueryFilter(wi => !wi.IsDeleted);
        
        builder.Ignore(wi => wi.DomainEvents);
    }
}
