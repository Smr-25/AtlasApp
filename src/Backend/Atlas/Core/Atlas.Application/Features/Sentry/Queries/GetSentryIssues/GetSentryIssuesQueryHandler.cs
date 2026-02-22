using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Sentry.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Sentry.Queries.GetSentryIssues;

public class GetSentryIssuesQueryHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    ISentryAdapter sentryAdapter
) : IRequestHandler<GetSentryIssuesQuery, List<SentryIssueDto>>
{
    public async Task<List<SentryIssueDto>> Handle(GetSentryIssuesQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken);
        return await sentryAdapter.GetIssuesAsync(token, request.ProjectSlug, cancellationToken);
    }
}

