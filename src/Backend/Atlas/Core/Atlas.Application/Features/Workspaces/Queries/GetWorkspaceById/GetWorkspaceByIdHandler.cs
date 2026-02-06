using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Workspaces.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspaceById;

public class GetWorkspaceByIdHandler(
    IApplicationDbContext context,
    IMapper mapper,
    ICurrentUserService currentUserService) : IRequestHandler<GetWorkspaceByIdQuery, WorkspaceDto>
{
    public async Task<WorkspaceDto> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var workspace = await context.Workspaces
            .Where(w => w.Id == request.WorkspaceId && w.UserProfileId == userId && !w.IsDeleted)
            .ProjectTo<WorkspaceDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        return workspace;
    }
}

