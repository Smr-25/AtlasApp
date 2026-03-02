using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("WorkspaceMembers", "atlas");

        builder.HasKey(wm => wm.Id);

        builder.Property(wm => wm.Id)
            .ValueGeneratedNever();

        builder.Property(wm => wm.CreatedAt)
            .IsRequired();

        builder.Property(wm => wm.ModifiedAt);

        builder.Property(wm => wm.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(wm => wm.WorkspaceId)
            .IsRequired();

        builder.Property(wm => wm.UserId)
            .IsRequired();

        builder.Property(wm => wm.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(wm => wm.JoinedAt)
            .IsRequired();

        builder.HasIndex(wm => wm.WorkspaceId)
            .HasDatabaseName("IX_WorkspaceMembers_WorkspaceId");

        builder.HasIndex(wm => wm.UserId)
            .HasDatabaseName("IX_WorkspaceMembers_UserId");

        builder.HasIndex(wm => new { wm.WorkspaceId, wm.UserId })
            .HasDatabaseName("IX_WorkspaceMembers_WorkspaceId_UserId")
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasOne(wm => wm.Workspace)
            .WithMany(w => w.Members)
            .HasForeignKey(wm => wm.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(wm => !wm.IsDeleted);

        builder.Ignore(wm => wm.DomainEvents);
    }
}

