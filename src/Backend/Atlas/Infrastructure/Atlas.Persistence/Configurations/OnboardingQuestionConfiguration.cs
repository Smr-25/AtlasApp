using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class OnboardingQuestionConfiguration : IEntityTypeConfiguration<OnboardingQuestion>
{
    private static readonly Guid Question1Id = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Question2Id = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid Question3Id = new("33333333-3333-3333-3333-333333333333");
    private static readonly DateTimeOffset SeedDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public void Configure(EntityTypeBuilder<OnboardingQuestion> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Text).IsRequired().HasMaxLength(500);
        builder.Property(q => q.TargetProfession);
        builder.HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasData(
            new 
            { 
                Id = Question1Id, 
                Text = "What is your profession?", 
                Order = 1, 
                IsMultiSelect = false,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new 
            { 
                Id = Question2Id, 
                Text = "What are your main goals for using Atlas?", 
                Order = 2, 
                IsMultiSelect = true,
                CreatedAt = SeedDate,
                IsDeleted = false
            },
            new 
            { 
                Id = Question3Id, 
                Text = "Which tools do you currently use in your workflow?", 
                Order = 3, 
                IsMultiSelect = true,
                CreatedAt = SeedDate,
                IsDeleted = false
            }
        );

    }
}