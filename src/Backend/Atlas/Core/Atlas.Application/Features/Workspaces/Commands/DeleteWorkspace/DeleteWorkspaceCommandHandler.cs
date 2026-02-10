using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Commands.DeleteWorkspace;

public class DeleteWorkspaceCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    ILogger<DeleteWorkspaceCommandHandler> logger) : IRequestHandler<DeleteWorkspaceCommand>
{
    public async Task Handle(DeleteWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Deleting workspace {WorkspaceId} for user {UserId}", request.WorkspaceId, userId);
        
        var workspace = await context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && w.UserProfileId == userId, cancellationToken);

        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        if (workspace.IsDefault)
        {
            logger.LogWarning("Attempted to delete default workspace {WorkspaceId}", request.WorkspaceId);
            throw new BusinessRuleViolationException("Delete", "Cannot delete default workspace.");
        }

        workspace.Delete(); 
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully deleted workspace {WorkspaceId}", request.WorkspaceId);
    }
}

