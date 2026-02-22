using Atlas.Application.Features.Sentry.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface ISentryAdapter
{
    Task<List<SentryIssueDto>> GetIssuesAsync(string accessToken, string projectSlug, CancellationToken ct);
    Task<SentryIssueDetailDto> GetIssueDetailAsync(string accessToken, string issueId, CancellationToken ct);
    Task ResolveIssueAsync(string accessToken, string issueId, CancellationToken ct);
}

