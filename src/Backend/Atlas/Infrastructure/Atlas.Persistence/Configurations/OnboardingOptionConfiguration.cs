using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Persistence.Configurations;

public class OnboardingOptionConfiguration : IEntityTypeConfiguration<OnboardingOption>
{
    public void Configure(EntityTypeBuilder<OnboardingOption> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Text).IsRequired().HasMaxLength(200);
        builder.Property(o => o.QuestionId).IsRequired();
        
        
        
    }
}