using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Personas.Commands.ActivatePersona;

public class ActivatePersonaCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<ActivatePersonaCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(ActivatePersonaCommand request, CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .FirstOrDefaultAsync(p => p.UserId.Equals(currentUserService.UserId), cancellationToken);
        if (persona == null)
            throw new NotFoundException("Persona not found");
        persona.Activate();
        await applicationDbContext.SaveChangesAsync();
        return ResponseModel<bool>.Success(true);
    }
}