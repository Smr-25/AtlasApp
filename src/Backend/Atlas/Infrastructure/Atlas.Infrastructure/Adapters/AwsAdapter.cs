using System.Net.Http.Headers;
using System.Net.Http.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Aws.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class AwsAdapter(IHttpClientFactory httpClientFactory, ILogger<AwsAdapter> logger) : IAwsAdapter
{
    public async Task<List<AwsDeploymentDto>> GetDeploymentsAsync(string accessToken, string serviceName, CancellationToken ct)
    {
        logger.LogInformation("Fetching AWS deployments for {Service}", serviceName);
        await Task.CompletedTask;
        return [];
    }

    public async Task<AwsDeploymentStatusDto> GetDeploymentStatusAsync(string accessToken, string deploymentId, CancellationToken ct)
    {
        logger.LogInformation("Fetching deployment status for {DeploymentId}", deploymentId);
        await Task.CompletedTask;
        return new AwsDeploymentStatusDto(deploymentId, "Unknown", 0, null);
    }

    public async Task<string> TriggerDeployAsync(string accessToken, string serviceName, string commitSha, CancellationToken ct)
    {
        logger.LogInformation("Triggering deploy for {Service} with commit {Sha}", serviceName, commitSha);
        await Task.CompletedTask;
        return Guid.NewGuid().ToString();
    }
}

