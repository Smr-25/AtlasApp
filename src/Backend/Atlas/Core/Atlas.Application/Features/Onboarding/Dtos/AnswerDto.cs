namespace Atlas.Application.Features.Onboarding.Dtos;

public record AnswerDto(Guid QuestionId, Guid OptionId, string? CustomValue = null);
