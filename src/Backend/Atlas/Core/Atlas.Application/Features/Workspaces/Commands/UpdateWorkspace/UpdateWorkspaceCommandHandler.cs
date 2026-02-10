using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Commands.UpdateWorkspace;

public class UpdateWorkspaceCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    ILogger<UpdateWorkspaceCommandHandler> logger) : IRequestHandler<UpdateWorkspaceCommand>
{
    public async Task Handle(UpdateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Updating workspace {WorkspaceId} for user {UserId}", request.WorkspaceId, userId);
        
        var workspace = await applicationDbContext.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && w.UserProfileId == userId, cancellationToken);

        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        workspace.UpdateDetails(request.Name, request.Description);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully updated workspace {WorkspaceId}", request.WorkspaceId);
    }
}

