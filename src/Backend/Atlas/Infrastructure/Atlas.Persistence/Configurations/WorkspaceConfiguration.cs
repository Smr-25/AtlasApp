using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("Workspaces", "atlas");
        
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Id)
            .ValueGeneratedNever();
        
        builder.Property(w => w.CreatedAt)
            .IsRequired();
        
        builder.Property(w => w.ModifiedAt);
        
        builder.Property(w => w.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(w => w.PersonaId)
            .IsRequired();
        
        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(w => w.Description)
            .HasMaxLength(500);
        
        builder.Property(w => w.Icon)
            .HasMaxLength(50); 
        
        builder.Property(w => w.Color)
            .HasMaxLength(9); 
        
        builder.Property(w => w.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(w => w.Config)
            .HasColumnType("jsonb");
        
        builder.Property(w => w.LastAccessedAt);
        
        builder.HasIndex(w => w.PersonaId)
            .HasDatabaseName("IX_Workspaces_PersonaId");
        
        builder.HasIndex(w => new { w.PersonaId, w.Name })
            .HasDatabaseName("IX_Workspaces_PersonaId_Name")
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false"); 
        
        builder.HasIndex(w => new { w.PersonaId, w.IsDefault })
            .HasDatabaseName("IX_Workspaces_PersonaId_IsDefault")
            .HasFilter("\"IsDefault\" = true");
        
        builder.HasIndex(w => w.LastAccessedAt)
            .HasDatabaseName("IX_Workspaces_LastAccessedAt");
        
        builder.HasMany(w => w.WorkspaceIntegrations)
            .WithOne(wi => wi.Workspace)
            .HasForeignKey(wi => wi.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasQueryFilter(w => !w.IsDeleted);
        
        builder.Ignore(w => w.DomainEvents);
    }
}
