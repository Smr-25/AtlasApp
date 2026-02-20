using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions", "atlas");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.Tier)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.StripeCustomerId)
            .HasMaxLength(255);

        builder.Property(s => s.StripeSubscriptionId)
            .HasMaxLength(255);

        builder.Property(s => s.MaxWorkspaces)
            .IsRequired();

        builder.Property(s => s.MaxIntegrations)
            .IsRequired();

        builder.Property(s => s.HasCustomHotkeys)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.ModifiedAt);

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("IX_Subscriptions_UserId")
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(s => s.StripeCustomerId)
            .HasDatabaseName("IX_Subscriptions_StripeCustomerId")
            .HasFilter("\"StripeCustomerId\" IS NOT NULL");

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.Ignore(s => s.DomainEvents);
        builder.Ignore(s => s.IsActive);
    }
}

