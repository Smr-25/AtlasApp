using Atlas.Domain.Enums;

namespace Atlas.Application.Features.AuditLogs.Dtos;

public record AuditLogDto(
    Guid Id,
    AuditAction Action,
    string Description,
    string? EntityName,
    Guid? EntityId,
    string? IpAddress,
    string? UserAgent,
    Guid? WorkspaceId,
    DateTimeOffset CreatedAt
);

public record ActiveSessionDto(
    string IpAddress,
    string UserAgent,
    DateTime LastLoginAt,
    bool IsCurrent
);

