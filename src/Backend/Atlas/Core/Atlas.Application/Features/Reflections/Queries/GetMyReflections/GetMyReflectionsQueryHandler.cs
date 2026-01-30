using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Reflections.Queries.GetMyReflections;

public class GetMyReflectionsQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetReflectionsQuery, ResponseModel<PagedResult>>
{
    public async Task<ResponseModel<PagedResult>> Handle(GetReflectionsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}