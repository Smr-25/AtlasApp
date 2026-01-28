using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.PersonaStates.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.PersonaStates.Commands.UpdateEnergyLevel;

public class UpdateEnergyLevelCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<UpdateEnergyLevelCommand, ResponseModel<PersonaStateDto>>
{
    public async Task<ResponseModel<PersonaStateDto>> Handle(UpdateEnergyLevelCommand request,
        CancellationToken cancellationToken)
    {
        var personaState = await applicationDbContext.PersonaStates
            .FirstOrDefaultAsync(ps => ps.Persona.UserId.Equals(currentUserService.UserId), cancellationToken);

        if (personaState == null)
            throw new NotFoundException(nameof(PersonaState), currentUserService.UserId!);

        personaState.UpdateEnergyLevel(request.Level);

        await applicationDbContext.SaveChangesAsync();

        var dto = mapper.Map<PersonaStateDto>(personaState);
        return ResponseModel<PersonaStateDto>.Success(dto);
    }
}
