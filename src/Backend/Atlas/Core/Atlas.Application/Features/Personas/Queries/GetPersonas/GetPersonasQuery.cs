using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Personas.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Personas.Queries.GetPersonas;

public record GetPersonasQuery : IRequest<List<PersonaDto>>;

public class GetPersonasQueryHandler(IApplicationDbContext applicationDbContext,IMapper mapper)
    : IRequestHandler<GetPersonasQuery, List<PersonaDto>>
{
    public async Task<List<PersonaDto>> Handle(GetPersonasQuery request, CancellationToken cancellationToken)
    {
        return await applicationDbContext.Personas
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.IsPrimary)
            .ThenBy(p => p.Name)
            .ProjectTo<PersonaDto>(mapper.ConfigurationProvider) 
            .ToListAsync(cancellationToken);
    }
}