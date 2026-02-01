using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Accounts.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Queries;

public class GetProfessionsQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetProfessionsQuery, List<ProfessionDto>>
{
    public async Task<List<ProfessionDto>> Handle(GetProfessionsQuery request, CancellationToken cancellationToken)
    {
        return await applicationDbContext.Professions
            .AsNoTracking()
            .Select(p => new ProfessionDto(p.Id, p.Name, p.Description))
            .ToListAsync(cancellationToken);
    }
}