using Atlas.Application.Common.Models;
using Atlas.Application.Features.PersonaStates.Dtos;
using MediatR;

namespace Atlas.Application.Features.PersonaStates.Commands.UpdateEnergyLevel;

public record UpdateEnergyLevelCommand(
    int Level
) : IRequest<ResponseModel<PersonaStateDto>>;
