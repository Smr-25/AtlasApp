using MediatR;

namespace Atlas.Application.Features.Personas.Commands.UpdatePersona;

public record UpdatePersonaDetailCommand(Guid Id, string Name, string? Bio) : IRequest;