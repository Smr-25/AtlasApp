using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class WorkspaceIntegration : BaseEntity
{
    public Guid WorkspaceId { get; private set; }

    public Guid IntegrationId { get; private set; }
    public string? Config { get; private set; }
    public Workspace Workspace { get; private set; } = null!;

    public Integration Integration { get; private set; } = null!;

    private WorkspaceIntegration() { }

   
    public static WorkspaceIntegration Create(Guid workspaceId, Guid integrationId, string? config = null)
    {
        return new WorkspaceIntegration
        {
            WorkspaceId = workspaceId,
            IntegrationId = integrationId,
            Config = config
        };
    }

    public void UpdateConfig(string? config)
    {
        Config = config;
        SetModified();
    }
}