using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class FocusSessionConfiguration : IEntityTypeConfiguration<FocusSession>
{
    public void Configure(EntityTypeBuilder<FocusSession> builder)
    {
        builder.ToTable("FocusSessions", "atlas");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .ValueGeneratedNever();

        builder.Property(f => f.DurationMinutes)
            .IsRequired();

        builder.Property(f => f.BreakDurationMinutes)
            .IsRequired()
            .HasDefaultValue(5);

        builder.Property(f => f.Tag)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Work");

        builder.Property(f => f.SessionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(f => f.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(f => f.UserId)
            .IsRequired();

        builder.Property(f => f.WorkspaceId);

        builder.Property(f => f.StartedAt);
        builder.Property(f => f.PausedAt);
        builder.Property(f => f.CompletedAt);

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.ModifiedAt);

        builder.Property(f => f.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(f => f.UserId)
            .HasDatabaseName("IX_FocusSessions_UserId");

        builder.HasIndex(f => new { f.UserId, f.Status })
            .HasDatabaseName("IX_FocusSessions_UserId_Status");

        builder.HasIndex(f => new { f.UserId, f.CompletedAt })
            .HasDatabaseName("IX_FocusSessions_UserId_CompletedAt");

        builder.HasQueryFilter(f => !f.IsDeleted);

        builder.Ignore(f => f.DomainEvents);
    }
}

