using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class OnboardingQuestion : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; } 
    public bool IsMultiSelect { get; set; } 
    public Guid ProfessionId { get; set; }
    public Profession Profession { get; set; } = null!;

    public ICollection<OnboardingOption> Options { get; set; } = new List<OnboardingOption>();
}