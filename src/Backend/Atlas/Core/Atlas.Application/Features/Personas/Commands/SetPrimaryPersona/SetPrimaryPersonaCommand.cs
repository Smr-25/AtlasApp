using MediatR;

namespace Atlas.Application.Features.Personas.Commands.SetPrimaryPersona;

public record SetPrimaryPersonaCommand(Guid Id) : IRequest;