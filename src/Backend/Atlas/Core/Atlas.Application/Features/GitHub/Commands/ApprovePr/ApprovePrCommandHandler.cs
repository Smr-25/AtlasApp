using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.GitHub.Commands.ApprovePr;

public class ApprovePrCommandHandler(
    IApplicationDbContext applicationDbContext,
    IEncryptionService encryptionService,
    IGitIntegrationAdapter gitAdapter
) : IRequestHandler<ApprovePrCommand>
{
    public async Task Handle(ApprovePrCommand request, CancellationToken cancellationToken)
    {
        var integration = await applicationDbContext.Integrations.FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");
        
        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        
        await gitAdapter.ApprovePullRequestAsync(token, request.Owner, request.Repo, request.PrNumber, cancellationToken);
    }
}