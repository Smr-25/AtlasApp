using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Workspaces.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspacesByPersona;

public class GetWorkspacesByPersonaQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService) : IRequestHandler<GetWorkspacesByPersonaQuery, List<WorkspaceDto>>
{
    public async Task<List<WorkspaceDto>> Handle(GetWorkspacesByPersonaQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var isPersonaOwner = await applicationDbContext.Personas
            .AnyAsync(p => p.Id == request.PersonaId && p.UserId.Equals(userId), cancellationToken);
        
        if (!isPersonaOwner) return [];
        
        return await applicationDbContext.Workspaces
            .Where(w => w.PersonaId == request.PersonaId && !w.IsDeleted) 
            .OrderByDescending(w => w.IsDefault) 
            .ThenBy(w => w.Name)
            .Select(w => new WorkspaceDto(
                w.Id, w.Name, w.Description, w.Icon, w.Color, w.IsDefault))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}