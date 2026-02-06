using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class WorkspaceIntegration : BaseEntity
{
    public Guid WorkspaceId { get; set; }
    public Workspace Workspace { get; set; } = null!;

    public Guid IntegrationId { get; set; }
    public Integration Integration { get; set; } = null!;

    public string? SettingsJson { get; set; } 
}