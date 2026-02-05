using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Personas.Commands.UpdatePersona;

public class UpdatePersonaDetailCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService
) : IRequestHandler<UpdatePersonaDetailCommand>
{
    public async Task Handle(UpdatePersonaDetailCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var persona = await applicationDbContext.Personas
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.UserId == userId, cancellationToken);

        if (persona == null) throw new NotFoundException("Persona", request.Id);

        persona.UpdateProfile(request.Name, request.Bio);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}