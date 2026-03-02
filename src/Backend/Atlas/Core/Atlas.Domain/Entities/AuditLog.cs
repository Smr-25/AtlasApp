using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid UserId { get; private set; }
    public AuditAction Action { get; private set; }
    public string Description { get; private set; } = null!;
    public string? EntityName { get; private set; }
    public Guid? EntityId { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? MetadataJson { get; private set; }
    public Guid? WorkspaceId { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        Guid userId,
        AuditAction action,
        string description,
        string? entityName = null,
        Guid? entityId = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? metadataJson = null,
        Guid? workspaceId = null)
    {
        return new AuditLog
        {
            UserId = userId,
            Action = action,
            Description = description,
            EntityName = entityName,
            EntityId = entityId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            MetadataJson = metadataJson,
            WorkspaceId = workspaceId
        };
    }
}

