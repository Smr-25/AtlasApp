using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistance.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(20);
        builder.HasIndex(u => u.UserName)
            .IsUnique();
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(15);
        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique();
        
        builder.Property(u => u.Status)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.IsDeleted)
            .HasDefaultValue(false);
    }
}