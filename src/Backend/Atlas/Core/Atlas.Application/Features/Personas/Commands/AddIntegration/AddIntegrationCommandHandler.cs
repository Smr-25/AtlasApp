using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Personas.Commands.AddIntegration;

public class AddIntegrationCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<AddIntegrationCommand, Guid>
{
    public async Task<Guid> Handle(AddIntegrationCommand request, CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .Include(p => p.Integrations)
            .FirstOrDefaultAsync(p => p.Id == request.PersonaId, cancellationToken);

        if (persona == null)
            throw new NotFoundException(nameof(Persona), request.PersonaId);

        var integration = Integration.Create(
            personaId: persona.Id,
            provider: request.Provider,
            name: request.Name,
            metadata: request.Metadata
        );
        await applicationDbContext.Integrations.AddAsync(integration, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return integration.Id;
    }
}