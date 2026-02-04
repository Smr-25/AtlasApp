using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.AddIntegration;

public record AddPersonaIntegrationCommand(
    Guid PersonaId,
    string Name,
    IntegrationProvider Provider,
    string? Metadata
) : IRequest<Guid>;