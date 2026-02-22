using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.GitHub.Commands.RejectPr;

public class RejectPrCommandHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    IGitIntegrationAdapter gitAdapter
) : IRequestHandler<RejectPrCommand>
{
    public async Task Handle(RejectPrCommand request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        await gitAdapter.RejectPullRequestAsync(token, request.Owner, request.Repo, request.PrNumber, request.Reason, cancellationToken);
    }
}

