namespace Atlas.Application.Features.Workspaces.Dtos;

public record WorkspaceDto(
    Guid Id, 
    string Name, 
    string? Description, 
    string? Icon, 
    string? Color, 
    bool IsDefault
);