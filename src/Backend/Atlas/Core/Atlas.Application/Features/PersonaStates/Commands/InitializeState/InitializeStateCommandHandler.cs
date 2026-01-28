using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.PersonaStates.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Atlas.Application.Features.PersonaStates.Commands.InitializeState;

public class InitializeStateCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<InitializeStateCommand, ResponseModel<PersonaStateDto>>
{
    public async Task<ResponseModel<PersonaStateDto>> Handle(InitializeStateCommand request,
        CancellationToken cancellationToken)
    {
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