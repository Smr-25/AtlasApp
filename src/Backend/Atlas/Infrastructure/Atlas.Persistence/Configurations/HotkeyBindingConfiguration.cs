using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class HotkeyBindingConfiguration : IEntityTypeConfiguration<HotkeyBinding>
{
    public void Configure(EntityTypeBuilder<HotkeyBinding> builder)
    {
        builder.ToTable("HotkeyBindings", "atlas");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .ValueGeneratedNever();

        builder.Property(h => h.UserId)
            .IsRequired();

        builder.Property(h => h.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.KeyCombination)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.IsGlobal)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(h => h.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(h => h.CreatedAt)
            .IsRequired();

        builder.Property(h => h.ModifiedAt);

        builder.Property(h => h.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(h => h.UserId)
            .HasDatabaseName("IX_HotkeyBindings_UserId");

        builder.HasIndex(h => new { h.UserId, h.Action })
            .HasDatabaseName("IX_HotkeyBindings_UserId_Action")
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasQueryFilter(h => !h.IsDeleted);

        builder.Ignore(h => h.DomainEvents);
    }
}

