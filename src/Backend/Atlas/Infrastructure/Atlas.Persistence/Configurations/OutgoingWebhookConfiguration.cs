using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class OutgoingWebhookConfiguration : IEntityTypeConfiguration<OutgoingWebhook>
{
    public void Configure(EntityTypeBuilder<OutgoingWebhook> builder)
    {
        builder.ToTable("OutgoingWebhooks", "atlas");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).ValueGeneratedNever();
        builder.Property(w => w.UserId).IsRequired();
        builder.Property(w => w.Name).IsRequired().HasMaxLength(100);
        builder.Property(w => w.Url).IsRequired().HasMaxLength(500);
        builder.Property(w => w.Secret).HasMaxLength(256);
        builder.Property(w => w.Events).HasColumnType("jsonb");
        builder.Property(w => w.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(w => w.ConsecutiveFailures).IsRequired().HasDefaultValue(0);
        builder.Property(w => w.CreatedAt).IsRequired();
        builder.Property(w => w.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(w => w.UserId).HasDatabaseName("IX_OutgoingWebhooks_UserId");

        builder.HasQueryFilter(w => !w.IsDeleted);
        builder.Ignore(w => w.DomainEvents);
    }
}

