using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Personas.Commands.AddIntegration;

public class AddPersonaIntegrationCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<AddPersonaIntegrationCommand, Guid>
{
    public async Task<Guid> Handle(AddPersonaIntegrationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        var persona = await applicationDbContext.Personas
            .Include(p => p.Integrations)
            .FirstOrDefaultAsync(p => p.Id == request.PersonaId && p.UserId == userId, cancellationToken);

        if (persona == null)
            throw new NotFoundException(nameof(Persona), request.PersonaId);

        var integration = Integration.Create(
            personaId: persona.Id,
            provider: request.Provider,
            name: request.Name,
            metadata: request.Metadata
        );
        
        persona.AddIntegration(integration);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return integration.Id;
    }
}