namespace Atlas.Application.Features.Workspaces.Dtos;

public record ToggleIntegrationDto(Guid IntegrationId, bool Enable);