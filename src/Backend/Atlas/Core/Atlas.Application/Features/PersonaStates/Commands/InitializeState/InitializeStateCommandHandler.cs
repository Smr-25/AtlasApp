using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.PersonaStates.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.PersonaStates.Commands.InitializeState;

public class InitializeStateCommandHandler(
    IApplicationDbContext applicationDbContext,
    IMapper mapper) : IRequestHandler<InitializeStateCommand, ResponseModel<PersonaStateDto>>
{
    public async Task<ResponseModel<PersonaStateDto>> Handle(InitializeStateCommand request,
        CancellationToken cancellationToken)
    {
        var existingState = await applicationDbContext.PersonaStates
            .AnyAsync(ps => ps.PersonaId == request.PersonaId, cancellationToken);

        if (existingState)
            throw new AlreadyExistException("PersonaState already exists for this Persona.");
       
        var personaState = PersonaState.Create(
            request.PersonaId,
            request.LifePhase,
            request.MentalLoad);

        personaState.UpdateEnergyLevel(request.EnergyLevel);
        personaState.UpdateFocusLevel(request.FocusLevel);

        await applicationDbContext.PersonaStates.AddAsync(personaState, cancellationToken);
        await applicationDbContext.SaveChangesAsync();
        var dto = mapper.Map<PersonaStateDto>(personaState);
        return ResponseModel<PersonaStateDto>.Success(dto);
    }
}