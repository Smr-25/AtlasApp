using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class OnboardingAnswer : BaseEntity
{
    public Guid UserId { get; set; } 
    public Guid QuestionId { get; set; }
    public Guid OptionId { get; set; }
    public string? CustomValue { get; set; }
}