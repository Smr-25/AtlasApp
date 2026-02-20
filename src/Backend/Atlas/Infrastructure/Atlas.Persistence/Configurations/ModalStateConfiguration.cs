using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class ModalStateConfiguration : IEntityTypeConfiguration<ModalState>
{
    public void Configure(EntityTypeBuilder<ModalState> builder)
    {
        builder.ToTable("ModalStates", "atlas");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedNever();

        builder.Property(m => m.UserId)
            .IsRequired();

        builder.Property(m => m.ModalType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(m => m.HasBeenSeen)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.PayloadJson)
            .HasColumnType("jsonb");

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.ModifiedAt);

        builder.Property(m => m.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(m => new { m.UserId, m.HasBeenSeen })
            .HasDatabaseName("IX_ModalStates_UserId_HasBeenSeen")
            .HasFilter("\"HasBeenSeen\" = false");

        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.Ignore(m => m.DomainEvents);
    }
}

