using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Integrations.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Atlas.Application.Features.Integrations.Commands.ConnectIntegration;

public class ConnectIntegrationHandler(
    IApplicationDbContext context, 
    IEncryptionService encryptionService, 
    IMapper mapper,
    ICurrentUserService currentUserService) 
    : IRequestHandler<ConnectIntegrationCommand, IntegrationDto>
{
    public async Task<IntegrationDto> Handle(ConnectIntegrationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
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

        return mapper.Map<IntegrationDto>(integration);
    }
}