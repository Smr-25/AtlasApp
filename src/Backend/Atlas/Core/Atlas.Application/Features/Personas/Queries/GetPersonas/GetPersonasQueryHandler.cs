using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Personas.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Personas.Queries.GetPersonas;

public class GetPersonasQueryHandler(
    IApplicationDbContext applicationDbContext, 
    ICurrentUserService currentUserService,
    IMapper mapper)
    : IRequestHandler<GetPersonasQuery, List<PersonaDto>>
{
    public async Task<List<PersonaDto>> Handle(GetPersonasQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        return await applicationDbContext.Personas
            .AsNoTracking()
            .Where(p => p.UserId == userId && !p.IsDeleted)
            .OrderByDescending(p => p.IsPrimary)
            .ThenBy(p => p.Name)
            .ProjectTo<PersonaDto>(mapper.ConfigurationProvider) 
            .ToListAsync(cancellationToken);
    }
}