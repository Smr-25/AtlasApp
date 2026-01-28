using Atlas.Application.Common.Models;
using Atlas.Application.Features.PersonaStates.Dtos;
using MediatR;

namespace Atlas.Application.Features.PersonaStates.Commands.UpdateFocusLevel;

public record UpdateFocusLevelCommand(
    int Level
) : IRequest<ResponseModel<PersonaStateDto>>;
