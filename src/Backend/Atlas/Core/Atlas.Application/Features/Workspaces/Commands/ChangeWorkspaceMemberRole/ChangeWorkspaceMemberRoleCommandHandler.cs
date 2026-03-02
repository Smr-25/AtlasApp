using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Commands.ChangeWorkspaceMemberRole;

public class ChangeWorkspaceMemberRoleCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    IWorkspaceAccessService workspaceAccess,
    ILogger<ChangeWorkspaceMemberRoleCommandHandler> logger) : IRequestHandler<ChangeWorkspaceMemberRoleCommand>
{
    public async Task Handle(ChangeWorkspaceMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        await workspaceAccess.ValidateAccessAsync(request.WorkspaceId, userId, WorkspaceMemberRole.Admin, cancellationToken);
        
        var workspace = await context.Workspaces
            .Include(w => w.Members)
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && !w.IsDeleted, cancellationToken);
        
        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);
        
        workspace.ChangeMemberRole(request.UserId, request.NewRole);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Changed role of user {TargetUserId} to {NewRole} in workspace {WorkspaceId}", 
            request.UserId, request.NewRole, request.WorkspaceId);
    }
}
