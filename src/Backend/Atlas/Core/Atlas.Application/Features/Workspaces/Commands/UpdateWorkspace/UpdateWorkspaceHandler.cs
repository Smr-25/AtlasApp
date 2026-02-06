using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workspaces.Commands.UpdateWorkspace;

public class UpdateWorkspaceHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateWorkspaceCommand>
{
    public async Task Handle(UpdateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        var workspace = await applicationDbContext.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && w.UserProfileId == userId, cancellationToken);

        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        workspace.UpdateDetails(request.Name, request.Description);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}