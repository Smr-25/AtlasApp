using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.Workspaces.Commands.CreateWorkspace;

public class CreateWorkspaceCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService) : IRequestHandler<CreateWorkspaceCommand, Guid>
{
    public async Task<Guid> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        var workspace = Workspace.Create(request.Name, userId);
        if (!string.IsNullOrEmpty(request.Description)) workspace.UpdateDetails(request.Name, request.Description);

        await applicationDbContext.Workspaces.AddAsync(workspace, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return workspace.Id;
    }
}

