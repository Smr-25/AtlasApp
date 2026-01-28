using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.DeactivePersona;

public class DeactivePersonaCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<DeactivePersonaCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(DeactivePersonaCommand request, CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .FindAsync([currentUserService.UserId], cancellationToken);
        if (persona == null)
            throw new NotFoundException("Persona not found");
        persona.Deactivate();
        await applicationDbContext.SaveChangesAsync();
        return ResponseModel<bool>.Success(true);
    }
}