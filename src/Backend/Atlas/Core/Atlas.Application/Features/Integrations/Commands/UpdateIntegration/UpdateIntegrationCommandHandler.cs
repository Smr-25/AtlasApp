using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Integrations.Commands.UpdateIntegration;

public class UpdateIntegrationCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    ILogger<UpdateIntegrationCommandHandler> logger) : IRequestHandler<UpdateIntegrationCommand>
{
    public async Task Handle(UpdateIntegrationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Updating integration {IntegrationId} for user {UserId}", request.IntegrationId, userId);
        
        var integration = await applicationDbContext.Integrations
            .FirstOrDefaultAsync(i => i.Id == request.IntegrationId && i.UserProfileId == userId, cancellationToken);

        if (integration == null) throw new NotFoundException("Integration", request.IntegrationId);

        integration.Rename(request.Name);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully updated integration {IntegrationId} name to '{Name}'", request.IntegrationId, request.Name);
    }
}

