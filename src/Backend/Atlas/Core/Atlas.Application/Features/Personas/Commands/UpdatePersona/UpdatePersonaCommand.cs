using Atlas.Application.Common.Models;
using Atlas.Application.Features.Personas.Dtos;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.UpdatePersona;

public record UpdatePersonaCommand(
    string Name,
    string? Alias = null
) : IRequest<ResponseModel<PersonaDto>>;