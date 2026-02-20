using Atlas.Application.Features.Integrations.Dtos;

namespace Atlas.Application.Features.Workspaces.Dtos;

public record WorkspaceDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsDefault,
    string? LocalFolderPath,
    bool IsShared,
    List<IntegrationDto> ActiveIntegrations
);    