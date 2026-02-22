using Atlas.Application.Features.Aws.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IAwsAdapter
{
    Task<List<AwsDeploymentDto>> GetDeploymentsAsync(string accessToken, string serviceName, CancellationToken ct);
    Task<AwsDeploymentStatusDto> GetDeploymentStatusAsync(string accessToken, string deploymentId, CancellationToken ct);
    Task<string> TriggerDeployAsync(string accessToken, string serviceName, string commitSha, CancellationToken ct);
}

