using MediatR;

namespace Atlas.Application.Features.Workflows.Commands.CreateBranchFromIssue;

public record CreateBranchFromIssueCommand(
    Guid JiraIntegrationId,
    Guid GitIntegrationId,
    string IssueKey,
    string RepoOwner,
    string RepoName,
    string BaseBranch
) : IRequest<string>;