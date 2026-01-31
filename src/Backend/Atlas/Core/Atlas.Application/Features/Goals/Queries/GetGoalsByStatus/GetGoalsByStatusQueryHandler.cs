using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Goals.Queries.GetGoalsByStatus;

public class GetGoalsByStatusQueryHandler : IRequestHandler<GetGoalsByStatusQuery, ResponseModel<PagedResult>>
{
    public async Task<ResponseModel<PagedResult>> Handle(GetGoalsByStatusQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}