using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Integrations.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Integrations.Queries.GetIntegrationsByPersona;

public record GetIntegrationsByPersonaQuery(Guid PersonaId) : IRequest<List<IntegrationDto>>;

public class GetIntegrationsByPersonaQueryHandler(IApplicationDbContext applicationDbContext,ICurrentUserService currentUserService) : IRequestHandler<GetIntegrationsByPersonaQuery, List<IntegrationDto>>
{
    public async Task<List<IntegrationDto>> Handle(GetIntegrationsByPersonaQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var isOwner = await applicationDbContext.Personas
            .AnyAsync(p => p.Id == request.PersonaId && p.UserId.Equals(userId), cancellationToken);
        
        if (!isOwner) return [];
        
        return await applicationDbContext.Integrations
            .Where(i => i.PersonaId == request.PersonaId && !i.IsDeleted)
            .Select(i => new IntegrationDto(
                i.Id, 
                i.Name, 
                i.Provider.ToString(), 
                i.IsActive))
            .ToListAsync(cancellationToken);
    }
}