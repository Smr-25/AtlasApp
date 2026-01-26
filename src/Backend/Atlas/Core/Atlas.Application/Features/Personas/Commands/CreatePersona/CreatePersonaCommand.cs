using Atlas.Application.Common.Models;
using Atlas.Application.Features.Personas.Dtos;
using Atlas.Application.Models;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.CreatePersona;

public record CreatePersonaCommand(
    string Name,
    string? Alias = null
) : IRequest<ResponseModel<PersonaDto>>;