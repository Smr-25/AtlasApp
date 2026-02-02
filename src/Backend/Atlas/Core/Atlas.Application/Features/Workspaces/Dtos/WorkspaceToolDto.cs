namespace Atlas.Application.Features.Workspaces.Dtos;

public record WorkspaceToolDto(
    Guid LinkId,            
    Guid IntegrationId,
    string Name,            
    string Provider,        
    string? Config         
);