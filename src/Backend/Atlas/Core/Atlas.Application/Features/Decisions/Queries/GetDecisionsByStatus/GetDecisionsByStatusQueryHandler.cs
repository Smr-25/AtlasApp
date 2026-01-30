using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Decisions.Queries.GetDecisionsByStatus;

public class GetDecisionsByStatusQueryHandler : IRequestHandler<GetDecisionsByStatusQuery, ResponseModel<PagedResult>>
{
    public Task<ResponseModel<PagedResult>> Handle(GetDecisionsByStatusQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}