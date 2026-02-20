using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Commands.CreateWorkspace;

public class CreateWorkspaceCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    ISubscriptionGuardService subscriptionGuard,
    ILogger<CreateWorkspaceCommandHandler> logger) : IRequestHandler<CreateWorkspaceCommand, Guid>
{
    public async Task<Guid> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Creating workspace '{Name}' for user {UserId}", request.Name, userId);
        
        await subscriptionGuard.ThrowIfCannotCreateWorkspaceAsync(userId, cancellationToken);
        
        var workspace = Workspace.Create(request.Name, userId, localFolderPath: request.LocalFolderPath);
        if (!string.IsNullOrEmpty(request.Description)) workspace.UpdateDetails(request.Name, request.Description);

        await applicationDbContext.Workspaces.AddAsync(workspace, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully created workspace {WorkspaceId}", workspace.Id);
        return workspace.Id;
    }
}

