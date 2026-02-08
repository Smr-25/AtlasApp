using Atlas.Application.Features.GitHub.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IGitIntegrationAdapter : IIntegrationAdapter
{
    Task<List<GitWorkItemDto>> GetMyWorkItemsAsync(string accessToken, CancellationToken ct);
    Task<List<GitWorkItemDto>> GetReviewRequestsAsync(string accessToken, CancellationToken ct);
    Task<List<GitPipelineDto>> GetRepoPipelinesAsync(string accessToken, string owner, string repo, CancellationToken ct);
    
    Task ApprovePullRequestAsync(string accessToken, string owner, string repo, string prNumber, CancellationToken ct);
    Task MergePullRequestAsync(string accessToken, string owner, string repo, string prNumber, CancellationToken ct);
    Task RetryPipelineAsync(string accessToken, string owner, string repo, string runId, CancellationToken ct);
}