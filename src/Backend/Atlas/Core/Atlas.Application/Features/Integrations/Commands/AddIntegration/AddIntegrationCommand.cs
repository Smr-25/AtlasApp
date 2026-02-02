using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Integrations.Commands.AddIntegration;

public record AddIntegrationCommand(
    Guid PersonaId,
    IntegrationProvider Provider,
    string Name,
    string AccessToken,
    string? RefreshToken,
    DateTimeOffset? TokenExpiresAt
) : IRequest<Guid>;