using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Integrations.Commands.ReportFailure;

public class MarkIntegrationExpiredHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<MarkIntegrationExpiredCommand>
{
    public async Task Handle(MarkIntegrationExpiredCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var integration = await context.Integrations
            .FirstOrDefaultAsync(i => i.Id == request.IntegrationId && i.UserProfileId == userId, cancellationToken);

        if (integration == null) throw new NotFoundException("Integration", request.IntegrationId);

        integration.MarkAsExpired();

        await context.SaveChangesAsync(cancellationToken);
    }
}