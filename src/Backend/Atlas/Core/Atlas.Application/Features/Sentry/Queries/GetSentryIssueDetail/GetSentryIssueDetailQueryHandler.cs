using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Sentry.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Sentry.Queries.GetSentryIssueDetail;

public class GetSentryIssueDetailQueryHandler(
    IApplicationDbContext dbContext,
    IEncryptionService encryptionService,
    ISentryAdapter sentryAdapter
) : IRequestHandler<GetSentryIssueDetailQuery, SentryIssueDetailDto>
{
    public async Task<SentryIssueDetailDto> Handle(GetSentryIssueDetailQuery request, CancellationToken cancellationToken)
    {
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.Id == request.IntegrationId, cancellationToken);
        if (integration == null) throw new NotFoundException("Integration not found");

        var token = encryptionService.Decrypt(integration.EncryptedAccessToken!);
        return await sentryAdapter.GetIssueDetailAsync(token, request.IssueId, cancellationToken);
    }
}

