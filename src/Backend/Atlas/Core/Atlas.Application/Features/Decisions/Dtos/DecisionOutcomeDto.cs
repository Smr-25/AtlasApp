using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Decisions.Dtos;

public record DecisionOutcomeDto(
    OutcomeStatus Status,
    string? Description,
    bool WasExpected,
    string? LessonLearned,
    DateTime RecordedAt
);