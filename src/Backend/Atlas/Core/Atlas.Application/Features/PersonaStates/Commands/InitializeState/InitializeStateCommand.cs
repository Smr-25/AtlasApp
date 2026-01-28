using Atlas.Application.Common.Models;
using Atlas.Application.Features.PersonaStates.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.PersonaStates.Commands.InitializeState;

public record InitializeStateCommand(
    Guid PersonaId,
    LifePhase LifePhase,
    MentalLoadLevel MentalLoad,
    int EnergyLevel,
    int FocusLevel
) : IRequest<ResponseModel<PersonaStateDto>>;