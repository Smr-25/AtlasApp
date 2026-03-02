using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.ToTable("UserPreferences", "atlas");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.UserId).IsRequired();
        builder.Property(p => p.Language).IsRequired().HasMaxLength(10).HasDefaultValue("en");
        builder.Property(p => p.Theme).IsRequired().HasMaxLength(20).HasDefaultValue("system");
        builder.Property(p => p.Timezone).IsRequired().HasMaxLength(50).HasDefaultValue("UTC");
        builder.Property(p => p.EmailNotifications).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.PushNotifications).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.InboxAlerts).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.InboxApprovals).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.InboxMentions).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.InboxSystem).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.WeeklyDigest).IsRequired().HasDefaultValue(false);
        builder.Property(p => p.CustomSettingsJson).HasColumnType("jsonb");
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(p => p.UserId).IsUnique().HasDatabaseName("IX_UserPreferences_UserId");

        builder.HasQueryFilter(p => !p.IsDeleted);
        builder.Ignore(p => p.DomainEvents);
    }
}

