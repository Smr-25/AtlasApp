using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams", "atlas");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.OwnerUserId)
            .IsRequired();

        builder.Property(t => t.MaxMembers)
            .IsRequired()
            .HasDefaultValue(7);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.ModifiedAt);

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasMany(t => t.Members)
            .WithOne()
            .HasForeignKey(m => m.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.OwnerUserId)
            .HasDatabaseName("IX_Teams_OwnerUserId");

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.Ignore(t => t.DomainEvents);
    }
}

