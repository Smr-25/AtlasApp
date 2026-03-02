using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Commands.AddWorkspaceMember;

public class AddWorkspaceMemberCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    IWorkspaceAccessService workspaceAccess,
    ILogger<AddWorkspaceMemberCommandHandler> logger) : IRequestHandler<AddWorkspaceMemberCommand>
{
    public async Task Handle(AddWorkspaceMemberCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        // Only Owner or Admin can add members
        await workspaceAccess.ValidateAccessAsync(request.WorkspaceId, userId, WorkspaceMemberRole.Admin, cancellationToken);
        
        var workspace = await context.Workspaces
            .Include(w => w.Members)
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && !w.IsDeleted, cancellationToken);
        
        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);
        
        // Verify target user exists
        var targetUserExists = await context.UserProfiles.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!targetUserExists)
            throw new NotFoundException("User", request.UserId);
        
        workspace.AddMember(request.UserId, request.Role);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Added user {TargetUserId} as {Role} to workspace {WorkspaceId}", 
            request.UserId, request.Role, request.WorkspaceId);
    }
}

