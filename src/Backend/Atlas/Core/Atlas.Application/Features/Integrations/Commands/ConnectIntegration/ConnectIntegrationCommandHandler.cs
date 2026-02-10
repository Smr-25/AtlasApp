using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Integrations.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Integrations.Commands.ConnectIntegration;

public class ConnectIntegrationCommandHandler(
    IApplicationDbContext context, 
    IEncryptionService encryptionService, 
    IMapper mapper,
    ICurrentUserService currentUserService,
    ILogger<ConnectIntegrationCommandHandler> logger) 
    : IRequestHandler<ConnectIntegrationCommand, IntegrationDto>
{
    public async Task<IntegrationDto> Handle(ConnectIntegrationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Connecting {Provider} integration for user {UserId}", request.Provider, userId);
        
        var encAccess = encryptionService.Encrypt(request.AccessToken);
        var encRefresh = request.RefreshToken != null ? encryptionService.Encrypt(request.RefreshToken) : null;

        var integration = Integration.Create(
            userId,
            request.Name,
            request.Provider,
            encAccess,
            encRefresh,
            request.ExpiresAt,
            request.MetadataJson
        );

        await context.Integrations.AddAsync(integration, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully connected integration {IntegrationId} ({Provider})", integration.Id, request.Provider);
        return mapper.Map<IntegrationDto>(integration);
    }
}

