using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class AwsDeployment : BaseEntity
{
    public string ServiceName { get; private set; } = null!;
    public string Environment { get; private set; } = null!;
    public string CommitSha { get; private set; } = null!;
    public DeploymentStatus Status { get; private set; }
    public string? LogUrl { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? FinishedAt { get; private set; }
    public Guid IntegrationId { get; private set; }
    public Guid UserId { get; private set; }

    private AwsDeployment() { }

    public static AwsDeployment Create(
        Guid userId,
        Guid integrationId,
        string serviceName,
        string environment,
        string commitSha,
        string? logUrl)
    {
        return new AwsDeployment
        {
            UserId = userId,
            IntegrationId = integrationId,
            ServiceName = serviceName,
            Environment = environment,
            CommitSha = commitSha,
            Status = DeploymentStatus.Pending,
            LogUrl = logUrl,
            StartedAt = DateTime.UtcNow
        };
    }

    public void MarkInProgress()
    {
        Status = DeploymentStatus.InProgress;
        SetModified();
    }

    public void MarkSuccess()
    {
        Status = DeploymentStatus.Success;
        FinishedAt = DateTime.UtcNow;
        SetModified();
    }

    public void MarkFailed()
    {
        Status = DeploymentStatus.Failed;
        FinishedAt = DateTime.UtcNow;
        SetModified();
    }

    public void MarkCancelled()
    {
        Status = DeploymentStatus.Cancelled;
        FinishedAt = DateTime.UtcNow;
        SetModified();
    }
}

