using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Reflections.Queries.GetReflectionsByType;

public class GetReflectionsByTypeQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetReflectionsByTypeQuery, ResponseModel<PagedResult>>
{
    public Task<ResponseModel<PagedResult>> Handle(GetReflectionsByTypeQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();    
    }
}