using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Teams.Commands.ShareWorkspace;

public class ShareWorkspaceCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    ISubscriptionGuardService subscriptionGuard)
    : IRequestHandler<ShareWorkspaceCommand, bool>
{
    public async Task<bool> Handle(ShareWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        // Must have team features
        if (!await subscriptionGuard.HasTeamFeaturesAsync(userId, cancellationToken))
            throw new ForbiddenException("Shared workspaces require Team subscription.");

        var team = await dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException("Team", request.TeamId);

        if (team.OwnerUserId != userId)
            throw new ForbiddenException("Only the team owner can share workspaces.");

        var workspace = await dbContext.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && w.UserProfileId == userId, cancellationToken)
            ?? throw new NotFoundException("Workspace", request.WorkspaceId);

        workspace.SetShared(true);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

