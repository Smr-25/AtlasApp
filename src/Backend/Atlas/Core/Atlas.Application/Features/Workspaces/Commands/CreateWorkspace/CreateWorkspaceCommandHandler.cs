using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workspaces.Commands.CreateWorkspace;

public class CreateWorkspaceCommandHandler(IApplicationDbContext applicationDbContext,ICurrentUserService currentUserService)
    : IRequestHandler<CreateWorkspaceCommand, Guid>
{
    public async Task<Guid> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var isPersonaOwner = await applicationDbContext.Personas
            .AnyAsync(p => p.Id == request.PersonaId && p.UserId.Equals(userId), cancellationToken);
        
        if (!isPersonaOwner)
            throw new UnauthorizedAccessException("You do not have permission to create a workspace for this persona.");
        
        var workspace = Workspace.Create(
            request.PersonaId,
            request.Name,
            request.Description,
            request.Icon,
            request.Color,
            request.IsDefault
        );

        await applicationDbContext.Workspaces.AddAsync(workspace, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return workspace.Id;
    }
}