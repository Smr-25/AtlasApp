using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Onboarding.Dtos;

public record OnboardingQuestionDto(
    Guid Id, 
    string Text, 
    int Order,
    bool IsMultiSelect, 
    UserProfession? TargetProfession,
    List<OnboardingOptionDto> Options
);