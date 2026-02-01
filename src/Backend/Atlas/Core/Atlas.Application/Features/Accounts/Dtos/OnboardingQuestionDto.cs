namespace Atlas.Application.Features.Accounts.Dtos;

public record OnboardingQuestionDto(Guid Id, string Text, bool IsMultiSelect, List<OnboardingOptionDto> Options);