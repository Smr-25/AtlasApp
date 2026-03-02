using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("SupportTickets", "atlas");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.UserId).IsRequired();
        builder.Property(s => s.Type).IsRequired().HasConversion<string>().HasMaxLength(30);
        builder.Property(s => s.Status).IsRequired().HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.Subject).IsRequired().HasMaxLength(300);
        builder.Property(s => s.Body).IsRequired().HasMaxLength(5000);
        builder.Property(s => s.PageUrl).HasMaxLength(500);
        builder.Property(s => s.BrowserInfo).HasMaxLength(500);
        builder.Property(s => s.AdminReply).HasMaxLength(5000);
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(s => s.UserId).HasDatabaseName("IX_SupportTickets_UserId");
        builder.HasIndex(s => s.Status).HasDatabaseName("IX_SupportTickets_Status");

        builder.HasQueryFilter(s => !s.IsDeleted);
        builder.Ignore(s => s.DomainEvents);
    }
}

