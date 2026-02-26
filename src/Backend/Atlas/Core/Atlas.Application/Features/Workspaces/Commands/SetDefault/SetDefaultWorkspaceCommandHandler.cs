using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Commands.SetDefault;

public class SetDefaultWorkspaceCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    ILogger<SetDefaultWorkspaceCommandHandler> logger) 
    : IRequestHandler<SetDefaultWorkspaceCommand>
{
    public async Task Handle(SetDefaultWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Setting default workspace {WorkspaceId} for user {UserId}", request.WorkspaceId, userId);

        var targetWorkspace = await context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && w.UserProfileId == userId, cancellationToken);

        if (targetWorkspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        if (targetWorkspace.IsDefault)
        {
            logger.LogDebug("Workspace {WorkspaceId} is already the default", request.WorkspaceId);
            return;
        }

        var currentDefaults = await context.Workspaces
            .Where(w => w.UserProfileId == userId && w.IsDefault && w.Id != request.WorkspaceId)
            .ToListAsync(cancellationToken);

        foreach (var wd in currentDefaults)
        {
            wd.SetDefault(false);
            logger.LogDebug("Removed default status from workspace {WorkspaceId}", wd.Id);
        }

        targetWorkspace.SetDefault(true);
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully set workspace {WorkspaceId} as default", request.WorkspaceId);
    }
}