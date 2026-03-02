using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Workspaces.Dtos;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspaceMembers;

public class GetWorkspaceMembersQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    IWorkspaceAccessService workspaceAccess,
    UserManager<AppUser> userManager) : IRequestHandler<GetWorkspaceMembersQuery, List<WorkspaceMemberDto>>
{
    public async Task<List<WorkspaceMemberDto>> Handle(GetWorkspaceMembersQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        await workspaceAccess.ValidateAccessAsync(request.WorkspaceId, userId, cancellationToken: cancellationToken);

        var members = await context.WorkspaceMembers
            .Where(wm => wm.WorkspaceId == request.WorkspaceId && !wm.IsDeleted)
            .OrderBy(wm => wm.Role)
            .ThenBy(wm => wm.JoinedAt)
            .ToListAsync(cancellationToken);

        var result = new List<WorkspaceMemberDto>();
        foreach (var m in members)
        {
            var user = await userManager.FindByIdAsync(m.UserId.ToString());
            result.Add(new WorkspaceMemberDto(
                m.UserId,
                user?.FullName ?? user?.UserName ?? "Unknown",
                m.Role,
                m.JoinedAt
            ));
        }

        return result;
    }
}

