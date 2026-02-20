using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class SnippetConfiguration : IEntityTypeConfiguration<Snippet>
{
    public void Configure(EntityTypeBuilder<Snippet> builder)
    {
        builder.ToTable("Snippets", "atlas");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Code)
            .IsRequired();

        builder.Property(s => s.Language)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("text");

        builder.Property(s => s.Tags)
            .HasMaxLength(500);

        builder.Property(s => s.IsFavorite)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.ModifiedAt);

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("IX_Snippets_UserId");

        builder.HasIndex(s => new { s.UserId, s.IsFavorite })
            .HasDatabaseName("IX_Snippets_UserId_IsFavorite")
            .HasFilter("\"IsFavorite\" = true");

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.Ignore(s => s.DomainEvents);
    }
}

