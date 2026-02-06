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

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Description)
            .HasMaxLength(500);

        builder.Property(w => w.IsDefault)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(w => w.UserProfileId)
            .IsRequired();

        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.Property(w => w.ModifiedAt);

        builder.Property(w => w.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(w => w.UserProfileId)
            .HasDatabaseName("IX_Workspaces_UserProfileId");

        builder.HasIndex(w => new { w.UserProfileId, w.Name })
            .HasDatabaseName("IX_Workspaces_UserProfileId_Name")
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(w => new { w.UserProfileId, w.IsDefault })
            .HasDatabaseName("IX_Workspaces_UserProfileId_IsDefault")
            .HasFilter("\"IsDefault\" = true");

        builder.HasQueryFilter(w => !w.IsDeleted);

        builder.Ignore(w => w.DomainEvents);
    }
}

