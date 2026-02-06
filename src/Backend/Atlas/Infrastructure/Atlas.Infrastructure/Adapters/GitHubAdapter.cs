using System.Net.Http.Headers;
using System.Net.Http.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.GitHub.Dtos;
using Atlas.Domain.Enums;

namespace Atlas.Infrastructure.Adapters;

public class GitHubAdapter(IHttpClientFactory httpClientFactory) : IGitHubAdapter
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("GitHub");
    public IntegrationProvider Provider => IntegrationProvider.GitHub;
    
    public async Task<List<ExternalResourceDto>> SearchResourcesAsync(string accessToken, string query, CancellationToken ct)
    {
        SetupHeaders(accessToken);

        var url = string.IsNullOrEmpty(query)
            ? "https://api.github.com/user/repos?sort=updated&per_page=10&type=all"
            : $"https://api.github.com/search/repositories?q={Uri.EscapeDataString(query)}+user:@me&per_page=10";

        try 
        {
            if (string.IsNullOrEmpty(query))
            {
                var repos = await _httpClient.GetFromJsonAsync<List<GitHubRepoJson>>(url, ct);
                return repos?.Select(MapRepoToDto).ToList() ?? [];
            }
            else
            {
                var response = await _httpClient.GetFromJsonAsync<GitHubSearchRepoResponse>(url, ct);
                return response?.Items.Select(MapRepoToDto).ToList() ?? [];
            }
        }
        catch
        {
            return []; 
        }
    }

    public async Task<ExternalResourceDto> GetResourceDetailsAsync(string accessToken, string resourceId, CancellationToken ct)
    {
        SetupHeaders(accessToken);

        var repoUrl = $"https://api.github.com/repos/{resourceId}";
        var repo = await _httpClient.GetFromJsonAsync<GitHubRepoJson>(repoUrl, ct);
        if (repo == null) throw new Exception("Repository not found");

        var statsUrl = $"https://api.github.com/repos/{resourceId}/stats/participation";
        var stats = await _httpClient.GetFromJsonAsync<GitHubStatsJson>(statsUrl, ct);

        var langUrl = $"https://api.github.com/repos/{resourceId}/languages";
        var langs = await _httpClient.GetFromJsonAsync<Dictionary<string, int>>(langUrl, ct);
        var topLanguage = langs?.Keys.FirstOrDefault() ?? "Unknown";

        var last4WeeksCommits = stats?.Owner?.TakeLast(4).Sum() ?? 0;
        var graphData = string.Join(",", stats?.Owner?.TakeLast(12) ?? []); // Son 12 həftə qrafik üçün

        var metadata = new Dictionary<string, string>
        {
            { "Stars", repo.StargazersCount.ToString() },
            { "Forks", repo.ForksCount.ToString() },
            { "OpenIssues", repo.OpenIssuesCount.ToString() },
            { "DefaultBranch", repo.DefaultBranch },
            { "Language", topLanguage },
            { "LastMonthCommits", last4WeeksCommits.ToString() },
            { "CommitGraph", graphData }, // Frontend bunu parse edib qrafik çəkəcək
            { "SizeKB", repo.Size.ToString() }
        };

        return new ExternalResourceDto(
            repo.Id.ToString(),
            repo.Name,
            repo.Description ?? "",
            repo.HtmlUrl,
            "Repository",
            metadata
        );
    }

    public async Task<List<GitHubWorkItemDto>> GetMyPullRequestsAsync(string accessToken, CancellationToken ct)
    {
        const string query = "is:pr is:open author:@me archived:false sort:updated";
        return await SearchWorkItemsAsync(accessToken, query, ct);
    }

    public async Task<List<GitHubWorkItemDto>> GetPullRequestsReviewRequestedAsync(string accessToken, CancellationToken ct)
    {
        const string query = "is:pr is:open review-requested:@me archived:false sort:updated";
        return await SearchWorkItemsAsync(accessToken, query, ct);
    }

    public async Task<List<GitHubWorkItemDto>> GetMyIssuesAsync(string accessToken, CancellationToken ct)
    {
        const string query = "is:issue is:open assignee:@me archived:false sort:updated";
        return await SearchWorkItemsAsync(accessToken, query, ct);
    }

    public async Task<List<GitHubActionRunDto>> GetRepoWorkflowRunsAsync(string accessToken, string owner, string repo, CancellationToken ct)
    {
        SetupHeaders(accessToken);
        var url = $"https://api.github.com/repos/{owner}/{repo}/actions/runs?per_page=5";
        
        try 
        {
            var response = await _httpClient.GetFromJsonAsync<GitHubWorkflowResponse>(url, ct);
            if (response?.WorkflowRuns == null) return [];

            return response.WorkflowRuns.Select(r => new GitHubActionRunDto(
                r.Id,
                r.Name,
                r.Status,     
                r.Conclusion, 
                r.HeadBranch,
                r.HtmlUrl,
                r.CreatedAt
            )).ToList();
        }
        catch
        {
            return []; 
        }
    }
    
    private async Task<List<GitHubWorkItemDto>> SearchWorkItemsAsync(string accessToken, string query, CancellationToken ct)
    {
        SetupHeaders(accessToken);
        var url = $"https://api.github.com/search/issues?q={Uri.EscapeDataString(query)}&per_page=20";

        var response = await _httpClient.GetFromJsonAsync<GitHubSearchIssueResponse>(url, ct);

        if (response?.Items == null) return [];

        return response.Items.Select(item => new GitHubWorkItemDto(
            item.Id.ToString(),
            item.Title,
            ExtractRepoName(item.RepositoryUrl),
            item.HtmlUrl,
            item.PullRequest != null ? "PullRequest" : "Issue",
            item.State,
            item.User.Login,
            item.UpdatedAt,
            item.Draft,
            item.Comments
        )).ToList();
    }

    private void SetupHeaders(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Atlas-Desktop-App");
    }

    private ExternalResourceDto MapRepoToDto(GitHubRepoJson r)
    {
        var metadata = new Dictionary<string, string>
        {
            { "Stars", r.StargazersCount.ToString() },
            { "Language", r.Language ?? "Unknown" },
            { "Visibility", r.Visibility ?? (r.Private ? "Private" : "Public") }
        };

        return new ExternalResourceDto(
            r.Id.ToString(),
            r.Name,
            r.Description ?? "",
            r.HtmlUrl,
            "Repository",
            metadata
        );
    }

    private string ExtractRepoName(string apiUrl)
    {
        var parts = apiUrl.Split("/repos/");
        return parts.Length > 1 ? parts[1] : apiUrl;
    }
    
    private record GitHubSearchRepoResponse(List<GitHubRepoJson> Items);
    private record GitHubRepoJson(long Id, string Name, string FullName, string Description, string HtmlUrl, string Language, int StargazersCount, int ForksCount, int OpenIssuesCount, int Size, string DefaultBranch, string Visibility, bool Private);
    
    private record GitHubStatsJson(List<int> Owner, List<int> All);

    private record GitHubSearchIssueResponse(List<GitHubIssueJson> Items);
    private record GitHubIssueJson(long Id, string Title, string State, string HtmlUrl, string RepositoryUrl, GitHubUserJson User, DateTime UpdatedAt, bool Draft, int Comments, object? PullRequest);
    private record GitHubUserJson(string Login);

    private record GitHubWorkflowResponse(List<GitHubRunJson> WorkflowRuns);
    private record GitHubRunJson(long Id, string Name, string Status, string Conclusion, string HeadBranch, string HtmlUrl, DateTime CreatedAt);
}