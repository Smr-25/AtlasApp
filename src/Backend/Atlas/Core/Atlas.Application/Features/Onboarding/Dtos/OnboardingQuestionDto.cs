
namespace Atlas.Application.Features.Onboarding.Dtos;

public record OnboardingQuestionDto(
    Guid Id, 
    string Text, 
    bool IsMultiSelect, 
    List<OnboardingOptionDto> Options
);