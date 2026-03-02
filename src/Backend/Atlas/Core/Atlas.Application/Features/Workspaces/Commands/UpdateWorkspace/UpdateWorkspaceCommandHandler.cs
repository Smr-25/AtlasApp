using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Commands.UpdateWorkspace;

public class UpdateWorkspaceCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IWorkspaceAccessService workspaceAccess,
    ILogger<UpdateWorkspaceCommandHandler> logger) : IRequestHandler<UpdateWorkspaceCommand>
{
    public async Task Handle(UpdateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Updating workspace {WorkspaceId} for user {UserId}", request.WorkspaceId, userId);
        
        // Editor or above can update workspace details
        await workspaceAccess.ValidateAccessAsync(request.WorkspaceId, userId, WorkspaceMemberRole.Editor, cancellationToken);
        
        var workspace = await applicationDbContext.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && !w.IsDeleted, cancellationToken);

        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        workspace.UpdateDetails(request.Name, request.Description);
        workspace.SetLocalFolderPath(request.LocalFolderPath);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully updated workspace {WorkspaceId}", request.WorkspaceId);
    }
}
