namespace Atlas.Application.Features.Workspaces.Dtos;

public record WorkspaceDto(Guid Id, Guid UserProfileId, string Name, string? Description, DateTime CreatedAt);