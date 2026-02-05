using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.CreatePersona;

public record CreatePersonaCommand(
    string Name,
    PersonaType PersonaType,
    string? Bio,
    bool IsPrimary
) : IRequest<Guid>;