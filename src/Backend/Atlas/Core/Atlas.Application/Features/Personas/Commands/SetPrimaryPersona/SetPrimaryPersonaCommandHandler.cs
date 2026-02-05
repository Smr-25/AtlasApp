using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Personas.Commands.SetPrimaryPersona;

public class SetPrimaryPersonaCommandHandler(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService) : IRequestHandler<SetPrimaryPersonaCommand>
{
    public async Task Handle(SetPrimaryPersonaCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var userPersonas = await applicationDbContext.Personas
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);

        var targetPersona = userPersonas.FirstOrDefault(p => p.Id == request.Id);
        if (targetPersona == null) throw new NotFoundException("Persona", request.Id);

        foreach (var persona in userPersonas)
        {
            if (persona.Id == request.Id)
                persona.SetAsPrimary();
            else if (persona.IsPrimary) 
                persona.RemovePrimaryStatus();
            
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}