using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Webhooks.Dtos;

public record WebhookDto(
    Guid Id,
    string Name,
    string Url,
    WebhookEvent[] Events,
    bool IsActive,
    int ConsecutiveFailures,
    DateTime? LastDeliveredAt,
    Guid? WorkspaceId,
    DateTimeOffset CreatedAt
);

