using Atlas.Application.Common.Models;
using Atlas.Application.Features.PersonaStates.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.PersonaStates.Commands.UpdatePhase;

public record UpdatePhaseCommand(
    LifePhase NewPhase,
    string? Note
) : IRequest<ResponseModel<PersonaStateDto>>;