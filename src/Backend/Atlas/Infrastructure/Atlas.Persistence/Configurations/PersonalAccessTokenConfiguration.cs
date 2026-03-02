using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class PersonalAccessTokenConfiguration : IEntityTypeConfiguration<PersonalAccessToken>
{
    public void Configure(EntityTypeBuilder<PersonalAccessToken> builder)
    {
        builder.ToTable("PersonalAccessTokens", "atlas");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();
        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
        builder.Property(t => t.TokenHash).IsRequired().HasMaxLength(512);
        builder.Property(t => t.TokenPrefix).IsRequired().HasMaxLength(20);
        builder.Property(t => t.Scopes).HasColumnType("jsonb");
        builder.Property(t => t.IsRevoked).IsRequired().HasDefaultValue(false);
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(t => t.UserId).HasDatabaseName("IX_PersonalAccessTokens_UserId");
        builder.HasIndex(t => t.TokenHash).IsUnique().HasDatabaseName("IX_PersonalAccessTokens_TokenHash");

        builder.HasQueryFilter(t => !t.IsDeleted);
        builder.Ignore(t => t.DomainEvents);
    }
}

