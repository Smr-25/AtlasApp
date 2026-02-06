using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.DeleteWorkspace;

public record DeleteWorkspaceCommand(Guid WorkspaceId) : IRequest;
