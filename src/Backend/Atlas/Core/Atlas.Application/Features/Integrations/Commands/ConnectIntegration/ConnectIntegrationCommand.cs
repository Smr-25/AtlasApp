using Atlas.Application.Features.Integrations.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Integrations.Commands.ConnectIntegration;

public record ConnectIntegrationCommand(
    IntegrationProvider Provider,
    string Name,          
    string AccessToken,   
    string? RefreshToken, 
    DateTime? ExpiresAt,
    string? MetadataJson
) : IRequest<IntegrationDto>;