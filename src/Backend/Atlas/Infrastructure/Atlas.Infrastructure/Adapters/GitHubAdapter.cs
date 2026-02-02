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
    
    public async Task<List<ExternalResourceDto>> GetResourcesAsync(string accessToken, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/repos?sort=updated&per_page=50");
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.UserAgent.ParseAdd("Atlas-App"); 

        var response = await _httpClient.SendAsync(request, cancellationToken);
        
        if (!response.IsSuccessStatusCode) return []; 
        
        var repos = await response.Content.ReadFromJsonAsync<List<GitHubRepoDto>>(cancellationToken: cancellationToken);

        if (repos == null) return [];

        return repos.Select(r => new ExternalResourceDto(
            r.id.ToString(),
            r.name,
            r.description,
            r.html_url,
            "Repository"
        )).ToList();
    }
    private record GitHubRepoDto(long id, string name, string description, string html_url);
}