using Atlas.Application.Features.Jira.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IJiraAdapter : IIntegrationAdapter
{
    Task<List<JiraIssueDto>> GetMyIssuesAsync(string accessToken, string domainUrl, CancellationToken ct);
    
    Task<JiraIssueDto> GetIssueAsync(string accessToken, string domainUrl, string issueKey, CancellationToken ct);
    
    Task MoveIssueAsync(string accessToken, string domainUrl, string issueKey, string transitionId, CancellationToken ct);
    
    Task<List<JiraTransitionDto>> GetTransitionsAsync(string accessToken, string domainUrl, string issueKey, CancellationToken ct);
}