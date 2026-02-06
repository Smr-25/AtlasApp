using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Dashboard.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Dashboard.Queries.GetGitHubWidgets;

public class GetGitHubWidgetsHandler(
    IApplicationDbContext applicationDbContext,
    IEncryptionService encryptionService,
    IGitHubAdapter gitHubAdapter
) : IRequestHandler<GetGitHubWidgetsQuery, GitHubDashboardDto>
{
    public async Task<GitHubDashboardDto> Handle(GetGitHubWidgetsQuery request, CancellationToken cancellationToken)
    {
        var integration = await applicationDbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);

        if (integration == null) throw new Exception("Integration not found");
        
        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);

        var prTask = gitHubAdapter.GetMyPullRequestsAsync(token, cancellationToken);
        var reviewTask = gitHubAdapter.GetPullRequestsReviewRequestedAsync(token, cancellationToken);
        var issueTask = gitHubAdapter.GetMyIssuesAsync(token, cancellationToken);

        await Task.WhenAll(prTask, reviewTask, issueTask);

        return new GitHubDashboardDto(
            MyPullRequests: await prTask,
            ReviewRequested: await reviewTask,
            MyIssues: await issueTask
        );
    }
}