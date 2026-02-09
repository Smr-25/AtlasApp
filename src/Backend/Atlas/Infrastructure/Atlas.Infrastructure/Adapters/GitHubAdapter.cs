using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.GitHub.Dtos;
using Atlas.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;
public class GitHubAdapter(IHttpClientFactory httpClientFactory, ILogger<GitHubAdapter> logger)
    : IGitIntegrationAdapter
{
    private const string GitHubApiBaseUrl = "https://api.github.com";
    private const string UserAgent = "Atlas-App";

    public IntegrationProvider Provider => IntegrationProvider.GitHub;

    public Task<List<ExternalResourceDto>> SearchResourcesAsync(string accessToken, string query, CancellationToken ct)
    {
        logger.LogWarning("SearchResourcesAsync not yet implemented for GitHub");
        return Task.FromResult(new List<ExternalResourceDto>());
    }

    public Task<ExternalResourceDto> GetResourceDetailsAsync(string accessToken, string resourceId, CancellationToken ct)
    {
        logger.LogWarning("GetResourceDetailsAsync not yet implemented for GitHub");
        return Task.FromResult<ExternalResourceDto>(null!);
    }

    public async Task<List<GitWorkItemDto>> GetMyWorkItemsAsync(string accessToken, CancellationToken ct)
    {
        const string url = $"{GitHubApiBaseUrl}/search/issues?q=is:open+author:@me+archived:false&sort=updated";
        return await FetchAndMapIssuesAsync(accessToken, url, ct);
    }

    public async Task<List<GitWorkItemDto>> GetReviewRequestsAsync(string accessToken, CancellationToken ct)
    {
        const string url = $"{GitHubApiBaseUrl}/search/issues?q=is:pr+is:open+review-requested:@me+archived:false";
        return await FetchAndMapIssuesAsync(accessToken, url, ct);
    }

    public async Task<List<GitPipelineDto>> GetRepoPipelinesAsync(string accessToken, string owner, string repo, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(repo);
        
        var url = $"{GitHubApiBaseUrl}/repos/{owner}/{repo}/actions/runs?per_page=5";

        try
        {
            using var client = CreateAuthenticatedClient(accessToken);
            var response = await client.GetFromJsonAsync<GitHubWorkflowResponse>(url, ct);
            
            if (response?.WorkflowRuns is null or { Count: 0 })
            {
                logger.LogDebug("No workflow runs found for {Owner}/{Repo}", owner, repo);
                return [];
            }

            return response.WorkflowRuns.Select(MapToPipelineDto).ToList();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to fetch pipelines for {Owner}/{Repo}", owner, repo);
            return [];
        }
    }

    public async Task ApprovePullRequestAsync(string accessToken, string owner, string repo, string prNumber, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(repo);
        ArgumentException.ThrowIfNullOrWhiteSpace(prNumber);
        
        var url = $"{GitHubApiBaseUrl}/repos/{owner}/{repo}/pulls/{prNumber}/reviews";
        
        using var client = CreateAuthenticatedClient(accessToken);
        var response = await client.PostAsJsonAsync(url, new { @event = "APPROVE" }, ct);
        
        response.EnsureSuccessStatusCode();
        logger.LogInformation("Approved PR #{PrNumber} in {Owner}/{Repo}", prNumber, owner, repo);
    }

    public async Task MergePullRequestAsync(string accessToken, string owner, string repo, string prNumber, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(repo);
        ArgumentException.ThrowIfNullOrWhiteSpace(prNumber);
        
        var url = $"{GitHubApiBaseUrl}/repos/{owner}/{repo}/pulls/{prNumber}/merge";
        
        using var client = CreateAuthenticatedClient(accessToken);
        var response = await client.PutAsJsonAsync(url, new { merge_method = "squash" }, ct);
        
        response.EnsureSuccessStatusCode();
        logger.LogInformation("Merged PR #{PrNumber} in {Owner}/{Repo}", prNumber, owner, repo);
    }

    public async Task RetryPipelineAsync(string accessToken, string owner, string repo, string runId, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(repo);
        ArgumentException.ThrowIfNullOrWhiteSpace(runId);
        
        var url = $"{GitHubApiBaseUrl}/repos/{owner}/{repo}/actions/runs/{runId}/rerun";
        
        using var client = CreateAuthenticatedClient(accessToken);
        var response = await client.PostAsync(url, null, ct);
        
        response.EnsureSuccessStatusCode();
        logger.LogInformation("Retried workflow run {RunId} in {Owner}/{Repo}", runId, owner, repo);
    }

    public async Task CreateBranchAsync(string accessToken, string owner, string repo, string baseBranch, string newBranchName,
        CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(repo);
        ArgumentException.ThrowIfNullOrWhiteSpace(baseBranch);
        ArgumentException.ThrowIfNullOrWhiteSpace(newBranchName);

        using var client = CreateAuthenticatedClient(accessToken);

        var getRefUrl = $"{GitHubApiBaseUrl}/repos/{owner}/{repo}/git/ref/heads/{baseBranch}";
        
        GitHubRefResponse? baseRef;
        try
        {
            baseRef = await client.GetFromJsonAsync<GitHubRefResponse>(getRefUrl, ct);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Base branch '{BaseBranch}' not found in {Owner}/{Repo}", baseBranch, owner, repo);
            throw new InvalidOperationException($"Base branch '{baseBranch}' not found in repo '{repo}'.", ex);
        }

        if (string.IsNullOrEmpty(baseRef?.Object.Sha))
        {
            throw new InvalidOperationException("Could not retrieve SHA from base branch.");
        }

        var createRefUrl = $"{GitHubApiBaseUrl}/repos/{owner}/{repo}/git/refs";
        
        var body = new
        {
            @ref = $"refs/heads/{newBranchName}",
            sha = baseRef.Object.Sha
        };

        var response = await client.PostAsJsonAsync(createRefUrl, body, ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            logger.LogError("Failed to create branch '{NewBranchName}': {Error}", newBranchName, error);
            throw new InvalidOperationException($"Failed to create branch: {error}");
        }
        
        logger.LogInformation("Created branch '{NewBranchName}' from '{BaseBranch}' in {Owner}/{Repo}", newBranchName, baseBranch, owner, repo);
    }

    #region Private Methods

    private HttpClient CreateAuthenticatedClient(string accessToken)
    {
        var client = httpClientFactory.CreateClient("GitHub");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        return client;
    }

    private async Task<List<GitWorkItemDto>> FetchAndMapIssuesAsync(string accessToken, string url, CancellationToken ct)
    {
        try
        {
            using var client = CreateAuthenticatedClient(accessToken);
            var response = await client.GetFromJsonAsync<GitHubSearchIssueResponse>(url, ct);

            if (response?.Items is null or { Count: 0 })
            {
                return [];
            }

            return response.Items.Select(MapToWorkItemDto).ToList();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to fetch issues from GitHub: {Url}", url);
            return [];
        }
    }

    private static GitWorkItemDto MapToWorkItemDto(GitHubIssueItem item)
    {
        var isPullRequest = item.PullRequest is not null;
        var state = MapToWorkItemState(item.State, item.Draft, isPullRequest);
        
        return new GitWorkItemDto(
            Id: item.Number.ToString(),
            Title: item.Title,
            RepositoryName: ExtractRepoName(item.RepositoryUrl),
            Url: item.HtmlUrl,
            Type: isPullRequest ? WorkItemType.PullRequest : WorkItemType.Issue,
            State: state,
            AuthorName: item.User?.Login ?? "Unknown",
            AuthorAvatarUrl: item.User?.AvatarUrl ?? string.Empty,
            UpdatedAt: item.UpdatedAt,
            IsDraft: item.Draft,
            SourceBranch: item.Head?.Ref,
            TargetBranch: item.Base?.Ref,
            CommentCount: item.Comments,
            CiStatus: null 
        );
    }

    private static WorkItemState MapToWorkItemState(string state, bool isDraft, bool isPullRequest)
    {
        if (isDraft && isPullRequest)
            return WorkItemState.Draft;
            
        return state.ToLowerInvariant() switch
        {
            "open" => WorkItemState.Open,
            "closed" => WorkItemState.Closed,
            "merged" => WorkItemState.Merged,
            _ => WorkItemState.Open
        };
    }

    private static GitPipelineDto MapToPipelineDto(GitHubRun run)
    {
        var status = run.Status == "completed" ? run.Conclusion ?? "unknown" : run.Status;
        var duration = run.Status == "completed" && run.UpdatedAt.HasValue
            ? run.UpdatedAt.Value - run.CreatedAt
            : (TimeSpan?)null;

        return new GitPipelineDto(
            Id: run.Id.ToString(),
            Status: status,
            BranchName: run.HeadBranch,
            CommitMessage: run.HeadCommit?.Message ?? "No commit message",
            Url: run.HtmlUrl,
            StartedAt: run.CreatedAt,
            Duration: duration
        );
    }

    private static string ExtractRepoName(string? repositoryUrl)
    {
        if (string.IsNullOrWhiteSpace(repositoryUrl))
            return "Unknown";
            
        var parts = repositoryUrl.Split("/repos/", StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? parts[1] : repositoryUrl;
    }

    #endregion

    #region GitHub API Response DTOs

    private record GitHubSearchIssueResponse(
        [property: JsonPropertyName("items")] List<GitHubIssueItem>? Items
    );

    private record GitHubIssueItem(
        [property: JsonPropertyName("number")] int Number,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("state")] string State,
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("repository_url")] string? RepositoryUrl,
        [property: JsonPropertyName("user")] GitHubUser? User,
        [property: JsonPropertyName("updated_at")] DateTime UpdatedAt,
        [property: JsonPropertyName("draft")] bool Draft,
        [property: JsonPropertyName("pull_request")] object? PullRequest,
        [property: JsonPropertyName("comments")] int Comments,
        [property: JsonPropertyName("head")] GitHubBranchRef? Head,
        [property: JsonPropertyName("base")] GitHubBranchRef? Base
    );

    private sealed record GitHubUser(
        [property: JsonPropertyName("login")] string Login,
        [property: JsonPropertyName("avatar_url")] string? AvatarUrl
    );

    private record GitHubBranchRef(
        [property: JsonPropertyName("ref")] string? Ref
    );

    private record GitHubWorkflowResponse(
        [property: JsonPropertyName("workflow_runs")] List<GitHubRun>? WorkflowRuns
    );

    private record GitHubRun(
        [property: JsonPropertyName("id")] long Id,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("conclusion")] string? Conclusion,
        [property: JsonPropertyName("head_branch")] string HeadBranch,
        [property: JsonPropertyName("html_url")] string HtmlUrl,
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt,
        [property: JsonPropertyName("head_commit")] GitHubCommit? HeadCommit
    );

    private record GitHubCommit(
        [property: JsonPropertyName("message")] string? Message
    );

    private record GitHubRefResponse(
        [property: JsonPropertyName("ref")] string Ref,
        [property: JsonPropertyName("object")] GitHubRefObject Object
    );

    private record GitHubRefObject(
        [property: JsonPropertyName("sha")] string Sha
    );

    #endregion
}