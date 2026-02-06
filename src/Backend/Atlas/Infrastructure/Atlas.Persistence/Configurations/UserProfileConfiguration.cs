using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<AppUserProfile>
{
    public void Configure(EntityTypeBuilder<AppUserProfile> builder)
    {
        builder.ToTable("UserProfiles", "atlas");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.JobTitle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Bio)
            .HasMaxLength(500);

        builder.Property(p => p.Profession)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.ThemeColor)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("#007AFF");

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.ModifiedAt);

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasMany(p => p.Workspaces)
            .WithOne()
            .HasForeignKey(w => w.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Integrations)
            .WithOne()
            .HasForeignKey(i => i.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.Profession)
            .HasDatabaseName("IX_UserProfiles_Profession");

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Ignore(p => p.DomainEvents);
    }
}