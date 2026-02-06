using MediatR;

namespace Atlas.Application.Features.Integrations.Commands.ReconnectIntegration;

public record ReconnectIntegrationCommand(
    Guid IntegrationId,
    string AccessToken,   
    string? RefreshToken, 
    DateTime? ExpiresAt,
    string? MetadataJson
) : IRequest;
