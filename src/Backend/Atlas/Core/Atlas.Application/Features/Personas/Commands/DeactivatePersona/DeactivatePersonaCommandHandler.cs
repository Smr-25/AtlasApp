using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Personas.Commands.DeactivatePersona;

public class DeactivatePersonaCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<DeactivatePersonaCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(DeactivatePersonaCommand request, CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .FirstOrDefaultAsync(p => p.UserId.Equals(currentUserService.UserId), cancellationToken);
        if (persona == null)
            throw new NotFoundException("Persona not found");
        persona.Deactivate();
        await applicationDbContext.SaveChangesAsync();
        return ResponseModel<bool>.Success(true);
    }
}