using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Integrations.Commands.ReportFailure;

public class MarkIntegrationExpiredCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    ILogger<MarkIntegrationExpiredCommandHandler> logger)
    : IRequestHandler<MarkIntegrationExpiredCommand>
{
    public async Task Handle(MarkIntegrationExpiredCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Marking integration {IntegrationId} as expired for user {UserId}", request.IntegrationId, userId);

        var integration = await context.Integrations
            .FirstOrDefaultAsync(i => i.Id == request.IntegrationId && i.UserProfileId == userId, cancellationToken);

        if (integration == null) throw new NotFoundException("Integration", request.IntegrationId);

        integration.MarkAsExpired();
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully marked integration {IntegrationId} as expired", request.IntegrationId);
    }
}