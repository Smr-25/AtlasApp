using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class BountyBoardConfiguration : IEntityTypeConfiguration<BountyBoard>
{
    public void Configure(EntityTypeBuilder<BountyBoard> builder)
    {
        builder.ToTable("BountyBoards", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.RewardPoints).IsRequired();
        builder.Property(e => e.JiraIssueKey).HasMaxLength(50);
        builder.HasIndex(e => e.TeamId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

