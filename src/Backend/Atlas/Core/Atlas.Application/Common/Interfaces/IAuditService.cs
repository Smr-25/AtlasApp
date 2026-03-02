using Atlas.Domain.Enums;

namespace Atlas.Application.Common.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        Guid userId,
        AuditAction action,
        string description,
        string? entityName = null,
        Guid? entityId = null,
        string? metadataJson = null,
        Guid? workspaceId = null,
        CancellationToken cancellationToken = default);
}

