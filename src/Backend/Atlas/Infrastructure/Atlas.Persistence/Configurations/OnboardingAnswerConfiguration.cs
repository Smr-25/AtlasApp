using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class OnboardingAnswerConfiguration : IEntityTypeConfiguration<OnboardingAnswer>
{
    public void Configure(EntityTypeBuilder<OnboardingAnswer> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.QuestionId).IsRequired();
        builder.Property(a => a.OptionId).IsRequired();
    }
}