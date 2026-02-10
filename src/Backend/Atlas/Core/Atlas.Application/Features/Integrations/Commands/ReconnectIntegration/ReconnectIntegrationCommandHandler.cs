using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Integrations.Commands.ReconnectIntegration;

public class ReconnectIntegrationCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    IEncryptionService encryptionService,
    ILogger<ReconnectIntegrationCommandHandler> logger) 
    : IRequestHandler<ReconnectIntegrationCommand>
{
    public async Task Handle(ReconnectIntegrationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Reconnecting integration {IntegrationId} for user {UserId}", request.IntegrationId, userId);

        var integration = await context.Integrations
            .FirstOrDefaultAsync(i => i.Id == request.IntegrationId && i.UserProfileId == userId, cancellationToken);

        if (integration == null) throw new NotFoundException("Integration", request.IntegrationId);

        var encAccess = encryptionService.Encrypt(request.AccessToken);
        var encRefresh = request.RefreshToken != null ? encryptionService.Encrypt(request.RefreshToken) : null;
        
        if (!string.IsNullOrEmpty(request.MetadataJson))
        {
            integration.UpdateMetadata(request.MetadataJson);
            logger.LogDebug("Updated metadata for integration {IntegrationId}", request.IntegrationId);
        }
        
        integration.UpdateTokens(encAccess, encRefresh, request.ExpiresAt);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully reconnected integration {IntegrationId}", request.IntegrationId);
    }
}