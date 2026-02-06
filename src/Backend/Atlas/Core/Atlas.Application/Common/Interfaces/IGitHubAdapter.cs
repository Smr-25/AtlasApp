using Atlas.Application.Features.GitHub.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IGitHubAdapter : IIntegrationAdapter
{
    Task<List<GitHubWorkItemDto>> GetMyPullRequestsAsync(string accessToken, CancellationToken ct);
    Task<List<GitHubWorkItemDto>> GetPullRequestsReviewRequestedAsync(string accessToken, CancellationToken ct);
    Task<List<GitHubWorkItemDto>> GetMyIssuesAsync(string accessToken, CancellationToken ct);
    Task<List<GitHubActionRunDto>> GetRepoWorkflowRunsAsync(string accessToken, string owner, string repo, CancellationToken ct);
}



