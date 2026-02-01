using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class Interest : BaseEntity
{
    public string Name { get; set; } = string.Empty; 
    public ICollection<OnboardingQuestion> Questions { get; set; } = new List<OnboardingQuestion>();
}