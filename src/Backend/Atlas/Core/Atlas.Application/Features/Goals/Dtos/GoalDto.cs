using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Goals.Dtos;

public record GoalDto(
    Guid Id,
    string Title,
    string? Description,
    GoalStatus Status,
    int Priority,
    int ProgressPercentage,
    DateTime CreatedAt,
    DateTime? DueDate,
    DateTime? CompletedAt,
    int RelatedDecisionCount
);