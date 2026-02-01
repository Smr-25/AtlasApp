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

        builder.Property(i => i.PersonaId)
            .IsRequired();

        builder.Property(i => i.Provider)
            .IsRequired()
            .HasConversion<string>() 
            .HasMaxLength(50);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.EncryptedAccessToken)
            .HasMaxLength(2000);

        builder.Property(i => i.RefreshToken)
            .HasMaxLength(2000);

        builder.Property(i => i.TokenExpiresAt);

        builder.Property(i => i.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(i => i.LastUsedAt);

        builder.Property(i => i.Metadata)
            .HasColumnType("jsonb");

        builder.HasIndex(i => i.PersonaId)
            .HasDatabaseName("IX_Integrations_PersonaId");

        builder.HasIndex(i => new { i.PersonaId, i.Provider })
            .HasDatabaseName("IX_Integrations_PersonaId_Provider");

        builder.HasIndex(i => new { i.PersonaId, i.Provider, i.Name })
            .HasDatabaseName("IX_Integrations_PersonaId_Provider_Name")
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false"); 

        builder.HasIndex(i => i.IsActive)
            .HasDatabaseName("IX_Integrations_IsActive")
            .HasFilter("\"IsActive\" = true");

        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.Ignore(i => i.DomainEvents);
    }
}