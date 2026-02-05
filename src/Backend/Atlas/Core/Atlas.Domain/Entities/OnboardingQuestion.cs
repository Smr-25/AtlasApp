using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class OnboardingQuestion : BaseEntity
{
    public string Text { get; private set; } = string.Empty;
    public int Order { get; private set; } 
    public bool IsMultiSelect { get; private set; } 
    public UserProfession? TargetProfession { get; private set; }

    private readonly List<OnboardingOption> _options = [];
    public IReadOnlyCollection<OnboardingOption> Options => _options.AsReadOnly();

    public static OnboardingQuestion Create(string text, int order, bool isMultiSelect, UserProfession? targetProfession = null)
    {
        return new OnboardingQuestion
        {
            Text = text,
            Order = order,
            IsMultiSelect = isMultiSelect,
            TargetProfession = targetProfession
        };
    }
    
    public void AddOption(OnboardingOption option)
    {
        _options.Add(option);
    }

}