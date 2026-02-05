using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class OnboardingOption : BaseEntity
{
    public string Text { get; private set; } = string.Empty;
    public string? RecommendedIntegration { get; private set; }
    public string? RecommendedTemplate { get; private set; }    

    public Guid QuestionId { get; private set; }
    
    public static OnboardingOption Create(string text, string? recommendedIntegration = null, string? recommendedTemplate = null)
    {
        return new OnboardingOption
        {
            Text = text,
            RecommendedIntegration = recommendedIntegration,
            RecommendedTemplate = recommendedTemplate
        };
    }
}