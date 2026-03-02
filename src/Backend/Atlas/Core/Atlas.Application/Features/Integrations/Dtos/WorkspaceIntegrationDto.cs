using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Integrations.Dtos;

public record WorkspaceIntegrationDto(
    Guid IntegrationId,
    string IntegrationName,
    IntegrationProvider Provider,
    IntegrationScope Scope,
    bool Enabled,
    DateTime ConnectedAt
);