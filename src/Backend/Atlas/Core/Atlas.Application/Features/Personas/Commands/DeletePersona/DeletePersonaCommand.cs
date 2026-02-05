using MediatR;

namespace Atlas.Application.Features.Personas.Commands.DeletePersona;

public record DeletePersonaCommand(Guid Id) : IRequest;