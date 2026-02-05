using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Personas.Commands.CreatePersona;

public class CreatePersonaCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreatePersonaCommand, Guid>
{
    public async Task<Guid> Handle(CreatePersonaCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        if (request.IsPrimary)
        {
            var existingPrimary = await applicationDbContext.Personas
                .FirstOrDefaultAsync(p => p.UserId == userId && p.IsPrimary, cancellationToken);
            existingPrimary?.RemovePrimaryStatus();
        }
        
        var persona = Persona.Create(
            userId,
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