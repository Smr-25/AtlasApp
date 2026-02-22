namespace Atlas.Application.Features.Aws.Dtos;

public record AwsDeploymentDto(
    string DeploymentId,
    string ServiceName,
    string Environment,
    string CommitSha,
    string Status,
    string? LogUrl,
    DateTime StartedAt,
    DateTime? FinishedAt);

public record AwsDeploymentStatusDto(
    string DeploymentId,
    string Status,
    double ProgressPercent,
    string? ErrorMessage);

