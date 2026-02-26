namespace Atlas.Application.Features.Onboarding.Dtos;

public record OnboardingAnswerDto(
    Guid QuestionId,
    Guid OptionId,
    string OptionText,
    string? CustomValue
);

