using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Decisions.Dtos;

public record DecisionDetailDto(
    Guid Id,
    string Title,
    string? Description,
    DecisionStatus Status,
    DecisionPriority Priority,
    Guid? GoalId,
    DateTime CreatedAt,
    DecisionContextDto? Context,
    DecisionOutcomeDto? Outcome
    // List<ReflectionDto> Reflections
);
