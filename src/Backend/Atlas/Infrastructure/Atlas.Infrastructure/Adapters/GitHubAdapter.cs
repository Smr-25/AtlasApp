using System.Net.Http.Headers;
using System.Net.Http.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Domain.Enums;

namespace Atlas.Infrastructure.Adapters;

public class GitHubAdapter(IHttpClientFactory httpClientFactory) : IIntegrationAdapter
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("GitHub");

    public IntegrationProvider Provider => IntegrationProvider.GitHub;

    public async Task<List<ExternalResourceDto>> GetResourcesAsync(string accessToken,
        CancellationToken cancellationToken)
    {
        var request =
            new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/repos?sort=updated&per_page=50");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.UserAgent.ParseAdd("Atlas-App");

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode) return [];

        var repos = await response.Content.ReadFromJsonAsync<List<GitHubRepoDto>>(cancellationToken: cancellationToken);

        if (repos == null) return [];

        return repos.Select(r => new ExternalResourceDto(
            r.Id.ToString(),
            r.Name,
            BuildDescription(r),
            r.HtmlUrl,
            "Repository"
        )).ToList();
    }
    
    private string BuildDescription(GitHubRepoDto r)
    {
        var parts = new List<string>();
        
        if (!string.IsNullOrEmpty(r.Language)) parts.Add($"💻 {r.Language}");
        if (r.StargazersCount > 0) parts.Add($"⭐ {r.StargazersCount}");
        
        if (r.PushedAt.HasValue) 
            parts.Add($"📅 {r.PushedAt.Value:MMM dd}");
        if (!string.IsNullOrEmpty(r.Description)) parts.Add($"- {r.Description}");

        return string.Join(" • ", parts);
    }

    private record GitHubRepoDto(
        long Id,
        string Name,
        string Description,
        string HtmlUrl,
        string Language,
        int StargazersCount,
        DateTime? PushedAt
    );
}