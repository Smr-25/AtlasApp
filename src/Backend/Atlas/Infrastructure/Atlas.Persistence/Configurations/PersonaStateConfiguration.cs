using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class PersonaStateConfiguration : IEntityTypeConfiguration<PersonaState>
{
    public void Configure(EntityTypeBuilder<PersonaState> builder)
    {
        builder.ToTable("PersonaStates");

        builder.Property(ps => ps.CurrentPhase)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ps => ps.MentalLoad)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ps => ps.LastUpdatedAt)
            .IsRequired();

        builder.Property(ps => ps.PersonaId)
            .IsRequired();
        
        builder.HasIndex(ps => ps.PersonaId)
            .IsUnique();
    }
}