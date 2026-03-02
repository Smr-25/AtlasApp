using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.AddWorkspaceMember;

public record AddWorkspaceMemberCommand(
    Guid WorkspaceId,
    Guid UserId,
    WorkspaceMemberRole Role = WorkspaceMemberRole.Viewer
) : IRequest;

