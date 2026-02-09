using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Jira.Dtos;
using Atlas.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class JiraAdapter(IHttpClientFactory httpClientFactory, ILogger<JiraAdapter> logger) : IJiraAdapter
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Jira");
    
    public IntegrationProvider Provider => IntegrationProvider.Jira;
    
    public async Task<List<ExternalResourceDto>> SearchResourcesAsync(string accessToken, string query, CancellationToken cancellationToken)
    {
        var (token, domainUrl) = ParseTokenAndDomain(accessToken); 
        SetupHeaders(token);

        var jql = $"text ~ \"{query}\" ORDER BY lastViewed DESC";
        var url = $"{domainUrl}/rest/api/3/search?jql={Uri.EscapeDataString(jql)}&maxResults=10&fields=summary,status,issuetype,updated";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<JiraSearchResponse>(url, cancellationToken);

            if (response?.Issues == null || response.Issues.Count == 0)
            {
                logger.LogDebug("No Jira issues found for query: {Query}", query);
                return [];
            }

            logger.LogInformation("Found {Count} Jira issues for query: {Query}", response.Issues.Count, query);
            
            return response.Issues.Select(issue => new ExternalResourceDto(
                issue.Key,
                issue.Fields.Summary,
                $"Status: {issue.Fields.Status.Name} | Type: {issue.Fields.Issuetype.Name}",
                $"{domainUrl}/browse/{issue.Key}",
                "Issue",
                new Dictionary<string, string>
                {
                    ["status"] = issue.Fields.Status.Name,
                    ["type"] = issue.Fields.Issuetype.Name,
                    ["updated"] = issue.Fields.Updated?.ToString("O") ?? DateTime.UtcNow.ToString("O")
                }
            )).ToList();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to search Jira issues for query: {Query}", query);
            return [];
        }
    }

    public async Task<ExternalResourceDto> GetResourceDetailsAsync(string accessToken, string resourceId, CancellationToken cancellationToken)
    {
        var (token, domainUrl) = ParseTokenAndDomain(accessToken);
        SetupHeaders(token);

        var url = $"{domainUrl}/rest/api/3/issue/{resourceId}?fields=summary,description,status,issuetype,updated,priority,assignee";

        try
        {
            var issue = await _httpClient.GetFromJsonAsync<JiraIssueResponse>(url, cancellationToken);
            if (issue == null)
            {
                logger.LogWarning("Jira issue not found: {ResourceId}", resourceId);
                return null!;
            }

            logger.LogDebug("Retrieved Jira issue details: {IssueKey}", issue.Key);
            
            return new ExternalResourceDto(
                issue.Key,
                issue.Fields.Summary,
                issue.Fields.Description ?? "No description provided",
                $"{domainUrl}/browse/{issue.Key}",
                issue.Fields.Issuetype.Name,
                new Dictionary<string, string>
                {
                    ["status"] = issue.Fields.Status.Name,
                    ["priority"] = issue.Fields.Priority?.Name ?? "None",
                    ["assignee"] = issue.Fields.Assignee?.DisplayName ?? "Unassigned",
                    ["updated"] = issue.Fields.Updated?.ToString("O") ?? DateTime.UtcNow.ToString("O")
                }
            );
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to get Jira issue details: {ResourceId}", resourceId);
            return null!;
        }
    }

    public async Task<List<JiraIssueDto>> GetMyIssuesAsync(string accessToken, string domainUrl, CancellationToken ct)
    {
        SetupHeaders(accessToken);

        var jql = "assignee = currentUser() AND statusCategory != Done ORDER BY updated DESC";
        var url = $"{domainUrl}/rest/api/3/search?jql={Uri.EscapeDataString(jql)}&fields=summary,status,issuetype,assignee,priority";

        logger.LogDebug("Fetching user's Jira issues from: {Domain}", domainUrl);
        return await FetchIssuesAsync(url, domainUrl, ct);
    }

    public async Task<JiraIssueDto> GetIssueAsync(string accessToken, string domainUrl, string issueKey, CancellationToken ct)
    {
        SetupHeaders(accessToken);
        var url = $"{domainUrl}/rest/api/3/issue/{issueKey}?fields=summary,status,issuetype,assignee,priority";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<JiraIssueResponse>(url, ct);
            if (response == null)
            {
                logger.LogWarning("Jira issue not found: {IssueKey}", issueKey);
                return null!;
            }
            
            logger.LogDebug("Retrieved Jira issue: {IssueKey}", issueKey);
            return MapToDto(response, domainUrl);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to get Jira issue: {IssueKey}", issueKey);
            throw;
        }
    }

    public async Task MoveIssueAsync(string accessToken, string domainUrl, string issueKey, string transitionId, CancellationToken ct)
    {
        SetupHeaders(accessToken);
        var url = $"{domainUrl}/rest/api/3/issue/{issueKey}/transitions";

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, new { transition = new { id = transitionId } }, ct);
            response.EnsureSuccessStatusCode();
            
            logger.LogInformation("Moved Jira issue {IssueKey} to transition {TransitionId}", issueKey, transitionId);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to move Jira issue {IssueKey} to transition {TransitionId}", issueKey, transitionId);
            throw;
        }
    }

    public async Task<List<JiraTransitionDto>> GetTransitionsAsync(string accessToken, string domainUrl, string issueKey, CancellationToken ct)
    {
        SetupHeaders(accessToken);
        var url = $"{domainUrl}/rest/api/3/issue/{issueKey}/transitions";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<JiraTransitionsResponse>(url, ct);
            if (response?.Transitions == null)
            {
                logger.LogDebug("No transitions found for issue: {IssueKey}", issueKey);
                return [];
            }

            return response.Transitions.Select(t => new JiraTransitionDto(t.Id, t.Name, t.To?.Name ?? "Unknown")).ToList();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to get transitions for issue: {IssueKey}", issueKey);
            return [];
        }
    }

    #region Private Methods

    private static (string Token, string DomainUrl) ParseTokenAndDomain(string accessToken)
    {
        // Format: "base64token|https://your-domain.atlassian.net"
        var parts = accessToken.Split('|');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid access token format. Expected: 'token|domainUrl'", nameof(accessToken));
        }
        return (parts[0], parts[1].TrimEnd('/'));
    }

    private void SetupHeaders(string token)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private async Task<List<JiraIssueDto>> FetchIssuesAsync(string url, string domainUrl, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<JiraSearchResponse>(url, ct);
            if (response?.Issues == null || response.Issues.Count == 0)
            {
                logger.LogDebug("No issues found from Jira search");
                return [];
            }
            
            logger.LogDebug("Fetched {Count} issues from Jira", response.Issues.Count);
            return response.Issues.Select(i => MapToDto(i, domainUrl)).ToList();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to fetch Jira issues from: {Url}", url);
            return [];
        }
    }

    private static JiraIssueDto MapToDto(JiraIssueResponse issue, string domain) => new(
        issue.Key,
        issue.Fields.Summary,
        issue.Fields.Issuetype.Name,
        issue.Fields.Status.Name,
        issue.Fields.Assignee?.DisplayName ?? "Unassigned",
        issue.Fields.Priority?.Name ?? "Medium",
        $"{domain}/browse/{issue.Key}"
    );

    #endregion

    #region Internal Models

    private record JiraSearchResponse(
        [property: JsonPropertyName("issues")] List<JiraIssueResponse> Issues,
        [property: JsonPropertyName("total")] int Total
    );

    private record JiraIssueResponse(
        [property: JsonPropertyName("key")] string Key,
        [property: JsonPropertyName("fields")] JiraFields Fields
    );

    private record JiraFields(
        [property: JsonPropertyName("summary")] string Summary,
        [property: JsonPropertyName("description")] string? Description,
        [property: JsonPropertyName("status")] JiraStatus Status,
        [property: JsonPropertyName("issuetype")] JiraIssueType Issuetype,
        [property: JsonPropertyName("assignee")] JiraUser? Assignee,
        [property: JsonPropertyName("priority")] JiraPriority? Priority,
        [property: JsonPropertyName("updated")] DateTime? Updated
    );

    private record JiraStatus([property: JsonPropertyName("name")] string Name);
    
    private record JiraIssueType([property: JsonPropertyName("name")] string Name);
    
    private record JiraPriority([property: JsonPropertyName("name")] string Name);

    private record JiraUser([property: JsonPropertyName("displayName")] string DisplayName);

    private record JiraTransitionsResponse(
        [property: JsonPropertyName("transitions")] List<JiraTransition> Transitions
    );

    private record JiraTransition(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("to")] JiraStatus? To
    );

    #endregion
}