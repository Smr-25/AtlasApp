using System.Net.Http.Headers;
using System.Net.Http.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Sentry.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class SentryAdapter(IHttpClientFactory httpClientFactory, ILogger<SentryAdapter> logger) : ISentryAdapter
{
    private const string BaseUrl = "https://sentry.io/api/0";

    public async Task<List<SentryIssueDto>> GetIssuesAsync(string accessToken, string projectSlug, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        var url = $"{BaseUrl}/projects/{projectSlug}/issues/?query=is:unresolved";

        try
        {
            var issues = await client.GetFromJsonAsync<List<SentryApiIssue>>(url, ct) ?? [];
            return issues.Select(i => new SentryIssueDto(
                i.Id, i.Title, i.Culprit, i.Level, i.Count,
                i.ShortId, i.FirstSeen, i.LastSeen, i.Status == "resolved"
            )).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch Sentry issues for {Project}", projectSlug);
            return [];
        }
    }

    public async Task<SentryIssueDetailDto> GetIssueDetailAsync(string accessToken, string issueId, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        var url = $"{BaseUrl}/issues/{issueId}/";

        var issue = await client.GetFromJsonAsync<SentryApiIssueDetail>(url, ct)
                    ?? throw new InvalidOperationException("Issue not found");

        return new SentryIssueDetailDto(
            issue.Id, issue.Title, issue.Culprit, issue.Level,
            issue.LineNumber, issue.FileName, issue.StackTrace,
            issue.Count, issue.Project?.Name ?? "", issue.FirstSeen, issue.LastSeen);
    }

    public async Task ResolveIssueAsync(string accessToken, string issueId, CancellationToken ct)
    {
        using var client = CreateClient(accessToken);
        var url = $"{BaseUrl}/issues/{issueId}/";
        var response = await client.PutAsJsonAsync(url, new { status = "resolved" }, ct);
        response.EnsureSuccessStatusCode();
        logger.LogInformation("Resolved Sentry issue {IssueId}", issueId);
    }

    private HttpClient CreateClient(string accessToken)
    {
        var client = httpClientFactory.CreateClient("AtlasClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private record SentryApiIssue(string Id, string Title, string Culprit, string Level, int Count, string ShortId, DateTime FirstSeen, DateTime LastSeen, string Status);
    private record SentryApiIssueDetail(string Id, string Title, string Culprit, string Level, int LineNumber, string FileName, string StackTrace, int Count, SentryProject? Project, DateTime FirstSeen, DateTime LastSeen);
    private record SentryProject(string Name);
}

