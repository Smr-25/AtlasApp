using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications", "atlas");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedNever();
        builder.Property(n => n.UserId).IsRequired();
        builder.Property(n => n.Category).IsRequired().HasConversion<string>().HasMaxLength(30);
        builder.Property(n => n.Priority).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(300);
        builder.Property(n => n.Body).IsRequired().HasMaxLength(2000);
        builder.Property(n => n.ActionType).HasMaxLength(100);
        builder.Property(n => n.ActionPayloadJson).HasColumnType("jsonb");
        builder.Property(n => n.SourceEntity).HasMaxLength(100);
        builder.Property(n => n.IsRead).IsRequired().HasDefaultValue(false);
        builder.Property(n => n.CreatedAt).IsRequired();
        builder.Property(n => n.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(n => n.UserId).HasDatabaseName("IX_Notifications_UserId");
        builder.HasIndex(n => new { n.UserId, n.IsRead }).HasDatabaseName("IX_Notifications_UserId_IsRead");
        builder.HasIndex(n => new { n.UserId, n.Category }).HasDatabaseName("IX_Notifications_UserId_Category");

        builder.HasQueryFilter(n => !n.IsDeleted);
        builder.Ignore(n => n.DomainEvents);
    }
}

