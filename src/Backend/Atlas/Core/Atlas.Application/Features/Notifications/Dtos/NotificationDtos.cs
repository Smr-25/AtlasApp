using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Notifications.Dtos;

public record NotificationDto(
    Guid Id,
    NotificationCategory Category,
    NotificationPriority Priority,
    string Title,
    string Body,
    string? ActionType,
    string? ActionPayloadJson,
    string? SourceEntity,
    Guid? SourceEntityId,
    bool IsRead,
    DateTime? ReadAt,
    Guid? WorkspaceId,
    DateTimeOffset CreatedAt
);

public record NotificationCountDto(
    int Total,
    int AlertsSecOps,
    int ApprovalsGit,
    int MentionsSocial,
    int SystemInsights
);

