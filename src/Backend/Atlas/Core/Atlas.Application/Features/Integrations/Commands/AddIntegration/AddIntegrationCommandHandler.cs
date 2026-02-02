using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Integrations.Commands.AddIntegration;

public class AddIntegrationCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IEncryptionService encryptionService) : IRequestHandler<AddIntegrationCommand, Guid>
{
    public async Task<Guid> Handle(AddIntegrationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var persona = await applicationDbContext.Personas
            .FirstOrDefaultAsync(p => p.Id == request.PersonaId && p.UserId.Equals(userId), cancellationToken);

        if (persona == null)
            throw new NotFoundException("Persona", request.PersonaId);
        
        var encryptedToken = encryptionService.Encrypt(request.AccessToken);
        var encryptedRefresh = request.RefreshToken != null ? encryptionService.Encrypt(request.RefreshToken) : null;
        
        var integration = Integration.Create(
            request.PersonaId,
            request.Provider,
            request.Name,
            encryptedToken,
            encryptedRefresh,
            request.TokenExpiresAt
        );
        await applicationDbContext.Integrations.AddAsync(integration, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return integration.Id;
    }
}