using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class LeaderModalStateConfiguration : IEntityTypeConfiguration<LeaderModalState>
{
    public void Configure(EntityTypeBuilder<LeaderModalState> builder)
    {
        builder.ToTable("LeaderModalStates", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.ModalType).IsRequired().HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.HasBeenSeen).IsRequired().HasDefaultValue(false);
        builder.Property(e => e.PayloadJson).HasColumnType("jsonb");
        builder.HasIndex(e => new { e.UserId, e.ModalType });
        builder.HasQueryFilter(e => !e.IsDeleted);
        builder.Ignore(e => e.DomainEvents);
    }
}

