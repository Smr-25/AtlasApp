using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.CreatePersona;

public class CreatePersonaCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreatePersonaCommand, Guid>
{
    public async Task<Guid> Handle(CreatePersonaCommand request, CancellationToken cancellationToken)
    {
        var persona = Domain.Entities.Persona.Create(
            Guid.NewGuid(),
            request.Name,
            request.PersonaType,
            request.Bio,
            request.IsPrimary
        );
        await applicationDbContext.Personas.AddAsync(persona, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return persona.Id;
    }
}