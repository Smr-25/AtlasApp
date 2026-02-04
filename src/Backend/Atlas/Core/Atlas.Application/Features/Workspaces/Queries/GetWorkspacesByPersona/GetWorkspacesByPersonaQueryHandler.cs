using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Workspaces.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspacesByPersona;

public class GetWorkspacesByPersonaQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetWorkspacesByPersonaQuery, List<WorkspaceDto>>
{
    public async Task<List<WorkspaceDto>> Handle(GetWorkspacesByPersonaQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var isPersonaOwner = await applicationDbContext.Personas
            .AnyAsync(p => p.Id == request.PersonaId && p.UserId.Equals(userId), cancellationToken);
        
        if (!isPersonaOwner) return [];
        
        return await applicationDbContext.Workspaces
            .AsNoTracking()
            .Where(w => w.PersonaId == request.PersonaId && !w.IsDeleted) 
            .OrderByDescending(w => w.IsDefault) 
            .ThenBy(w => w.Name)
            .ProjectTo<WorkspaceDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}