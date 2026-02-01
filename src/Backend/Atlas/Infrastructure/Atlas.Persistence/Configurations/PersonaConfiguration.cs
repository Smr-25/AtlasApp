using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;
public class PersonaConfiguration : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.ToTable("Personas", "atlas");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .ValueGeneratedNever(); 
        
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        
        builder.Property(p => p.ModifiedAt);
        
        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(p => p.UserId)
            .IsRequired();
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Bio)
            .HasMaxLength(500);
        
        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<string>() 
            .HasMaxLength(50);
        
        builder.Property(p => p.Config)
            .HasColumnType("jsonb");
        
        builder.Property(p => p.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.HasIndex(p => p.UserId)
            .HasDatabaseName("IX_Personas_UserId");
        
        builder.HasIndex(p => new { p.UserId, p.Type })
            .HasDatabaseName("IX_Personas_UserId_Type");
        
        builder.HasIndex(p => new { p.UserId, p.IsPrimary })
            .HasDatabaseName("IX_Personas_UserId_IsPrimary")
            .HasFilter("\"IsPrimary\" = true"); 
        
        builder.HasMany(p => p.Integrations)
            .WithOne(i => i.Persona)
            .HasForeignKey(i => i.PersonaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(p => p.Workspaces)
            .WithOne(w => w.Persona)
            .HasForeignKey(w => w.PersonaId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasQueryFilter(p => !p.IsDeleted);
        
        builder.Ignore(p => p.DomainEvents);
    }
}