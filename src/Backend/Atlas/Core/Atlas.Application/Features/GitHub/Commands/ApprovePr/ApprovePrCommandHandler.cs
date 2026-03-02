using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.GitHub.Commands.ApprovePr;

public class ApprovePrCommandHandler(
    IApplicationDbContext applicationDbContext,
    IEncryptionService encryptionService,
    IGitIntegrationAdapter gitAdapter,
    IAtlasHubService hubService,
    ICurrentUserService currentUser)
    : IRequestHandler<ApprovePrCommand>
{
    public async Task Handle(ApprovePrCommand request, CancellationToken cancellationToken)
    {
        var integration = await applicationDbContext.Integrations.FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");
        
        var token = encryptionService.Decrypt(integration.EncryptedAccessToken!);
        await gitAdapter.ApprovePullRequestAsync(token, request.Owner, request.Repo, request.PrNumber, cancellationToken);

        var userId = currentUser.GetUserIdOrDefault();
        var payload = new { request.IntegrationId, request.Owner, request.Repo, request.PrNumber, ApprovedBy = userId };

        if (userId != null)
            await hubService.SendToUserAsync(userId.Value, "FeedUpdated", payload, cancellationToken);
    }
}