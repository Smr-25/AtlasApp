using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class IntegrationConfiguration : IEntityTypeConfiguration<Integration>
{
    public void Configure(EntityTypeBuilder<Integration> builder)
    {
        builder.ToTable("Integrations", "atlas");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.ModifiedAt);

        builder.Property(i => i.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(i => i.UserProfileId)
            .IsRequired();

        builder.Property(i => i.Provider)
            .IsRequired()
            .HasConversion<string>() 
            .HasMaxLength(50);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(i => i.EncryptedAccessToken)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(i => i.EncryptedRefreshToken)
            .HasMaxLength(2000);

        builder.Property(i => i.TokenExpiresAt);

        builder.Property(i => i.MetadataJson)
            .HasColumnType("jsonb");

        builder.HasIndex(i => i.UserProfileId)
            .HasDatabaseName("IX_Integrations_UserProfileId");

        builder.HasIndex(i => new { i.UserProfileId, i.Provider })
            .HasDatabaseName("IX_Integrations_UserProfileId_Provider");

        builder.HasIndex(i => new { i.UserProfileId, i.Provider, i.Name })
            .HasDatabaseName("IX_Integrations_UserProfileId_Provider_Name")
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false"); 

        builder.HasIndex(i => i.Status)
            .HasDatabaseName("IX_Integrations_Status")
            .HasFilter("\"Status\" = 'Active'");

        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.Ignore(i => i.DomainEvents);
        builder.Ignore(i => i.WorkspaceConnections);
    }
}