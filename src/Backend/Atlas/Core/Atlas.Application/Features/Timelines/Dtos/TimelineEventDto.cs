using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Timelines.Dtos;

public record TimelineEventDto(
    Guid Id,
    TimelineEventType EventType,
    string Title,
    string? Description,
    Guid? RelatedEntityId,
    string? RelatedEntityType,
    DateTime OccurredAt
);