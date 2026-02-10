using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Integrations.Commands.DeleteIntegration;

public class DeleteIntegrationCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    ILogger<DeleteIntegrationCommandHandler> logger) : IRequestHandler<DeleteIntegrationCommand>
{
    public async Task Handle(DeleteIntegrationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Deleting integration {IntegrationId} for user {UserId}", request.IntegrationId, userId);
        
        var integration = await applicationDbContext.Integrations
            .FirstOrDefaultAsync(i => i.Id == request.IntegrationId && i.UserProfileId == userId, cancellationToken);

        if (integration == null) throw new NotFoundException("Integration", request.IntegrationId);

        integration.Delete();
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully deleted integration {IntegrationId}", request.IntegrationId);
    }
}