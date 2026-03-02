using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.ChangeWorkspaceMemberRole;

public record ChangeWorkspaceMemberRoleCommand(
    Guid WorkspaceId,
    Guid UserId,
    WorkspaceMemberRole NewRole
) : IRequest;

