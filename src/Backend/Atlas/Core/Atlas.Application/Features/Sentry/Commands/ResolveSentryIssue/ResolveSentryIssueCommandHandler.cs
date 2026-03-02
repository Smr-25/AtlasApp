using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Sentry.Commands.ResolveSentryIssue;

public class ResolveSentryIssueCommandHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    ISentryAdapter sentryAdapter
) : IRequestHandler<ResolveSentryIssueCommand>
{
    public async Task Handle(ResolveSentryIssueCommand request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken!);
        await sentryAdapter.ResolveIssueAsync(token, request.IssueId, cancellationToken);
    }
}

