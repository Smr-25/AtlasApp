using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class FigmaCommentConfiguration : IEntityTypeConfiguration<FigmaComment>
{
    public void Configure(EntityTypeBuilder<FigmaComment> builder)
    {
        builder.ToTable("FigmaComments", "atlas");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.FileKey).IsRequired().HasMaxLength(200);
        builder.Property(e => e.CommentId).IsRequired().HasMaxLength(100);
        builder.Property(e => e.AuthorName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Message).IsRequired().HasMaxLength(2000);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.IntegrationId);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

