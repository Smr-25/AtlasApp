using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.UpdateWorkspace;

public record UpdateWorkspaceCommand(Guid WorkspaceId, string Name, string? Description) : IRequest;
