using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.GitHub.Queries.GetDashboard;

public class GetGitDashboardHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService, 
    IGitIntegrationAdapter gitAdapter     
) : IRequestHandler<GetGitDashboardQuery, GitDashboardVm>
{
    public async Task<GitDashboardVm> Handle(GetGitDashboardQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);

        if (integration == null) throw new NotFoundException("Integration not found");
        if (integration.Provider != IntegrationProvider.GitHub) throw new Exception("This is not a Git integration");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);

        var myItemsTask = gitAdapter.GetMyWorkItemsAsync(token, cancellationToken);
        var reviewsTask = gitAdapter.GetReviewRequestsAsync(token, cancellationToken);

        await Task.WhenAll(myItemsTask, reviewsTask);

        return new GitDashboardVm(await myItemsTask, await reviewsTask);
    }
}