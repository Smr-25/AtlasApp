using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.ActivePersona;

public class ActivePersonaCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<ActivePersonaCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(ActivePersonaCommand request, CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .FindAsync([currentUserService.UserId], cancellationToken);
        if (persona == null)
            throw new NotFoundException("Persona not found");
        persona.Activate();
        await applicationDbContext.SaveChangesAsync();
        return ResponseModel<bool>.Success(true);
    }
}