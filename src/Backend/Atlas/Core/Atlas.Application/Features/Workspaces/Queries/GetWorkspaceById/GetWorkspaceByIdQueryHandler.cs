using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Workspaces.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspaceById;

public class GetWorkspaceByIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    ICurrentUserService currentUserService,
    ILogger<GetWorkspaceByIdQueryHandler> logger) : IRequestHandler<GetWorkspaceByIdQuery, WorkspaceDto>
{
    public async Task<WorkspaceDto> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogDebug("Fetching workspace {WorkspaceId} for user {UserId}", request.WorkspaceId, userId);

        var workspace = await context.Workspaces
            .Include(w => w.WorkspaceIntegrations)
            .ThenInclude(wi => wi.Integration)
            .Where(w => w.Id == request.WorkspaceId && w.UserProfileId == userId && !w.IsDeleted)
            .ProjectTo<WorkspaceDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        logger.LogDebug("Successfully retrieved workspace {WorkspaceId}", request.WorkspaceId);
        return workspace;
    }
}

