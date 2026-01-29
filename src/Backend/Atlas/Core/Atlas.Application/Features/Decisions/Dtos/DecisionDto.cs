using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Decisions.Dtos;

public record DecisionDto(
    Guid Id,
    string Title,
    string? Description,
    DecisionStatus Status,
    DecisionPriority Priority,
    Guid? GoalId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? ClosedAt,
    bool HasContext,
    bool HasOutcome,
    int ReflectionCount
);