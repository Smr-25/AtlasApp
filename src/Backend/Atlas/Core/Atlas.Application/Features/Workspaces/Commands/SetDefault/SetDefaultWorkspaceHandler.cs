using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workspaces.Commands.SetDefault;

public class SetDefaultWorkspaceHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) 
    : IRequestHandler<SetDefaultWorkspaceCommand>
{
    public async Task Handle(SetDefaultWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var targetWorkspace = await context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && w.UserProfileId == userId, cancellationToken);

        if (targetWorkspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        if (targetWorkspace.IsDefault) return;

        var currentDefault = await context.Workspaces
            .FirstOrDefaultAsync(w => w.UserProfileId == userId && w.IsDefault, cancellationToken);

        if (currentDefault != null)
        {
            currentDefault.SetDefault(false);
        }

        targetWorkspace.SetDefault(true);

        await context.SaveChangesAsync(cancellationToken);
    }
}