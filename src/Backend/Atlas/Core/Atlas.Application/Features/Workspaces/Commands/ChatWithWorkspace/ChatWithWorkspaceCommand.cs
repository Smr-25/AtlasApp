using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.ChatWithWorkspace;

public record ChatWithWorkspaceCommand(
    Guid WorkspaceId,
    string Message
) : IRequest<string>;