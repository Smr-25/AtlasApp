using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class OnboardingOption : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public string BioPart { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }
    public OnboardingQuestion Question { get; set; } = null!;
}