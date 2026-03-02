using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Commands.RemoveWorkspaceMember;

public class RemoveWorkspaceMemberCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    IWorkspaceAccessService workspaceAccess,
    ILogger<RemoveWorkspaceMemberCommandHandler> logger) : IRequestHandler<RemoveWorkspaceMemberCommand>
{
    public async Task Handle(RemoveWorkspaceMemberCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        // Owner or Admin can remove members; a member can remove themselves
        if (userId != request.UserId)
            await workspaceAccess.ValidateAccessAsync(request.WorkspaceId, userId, WorkspaceMemberRole.Admin, cancellationToken);

        var workspace = await context.Workspaces
            .Include(w => w.Members)
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && !w.IsDeleted, cancellationToken);
        
        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);
        
        workspace.RemoveMember(request.UserId);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Removed user {TargetUserId} from workspace {WorkspaceId}", 
            request.UserId, request.WorkspaceId);
    }
}

