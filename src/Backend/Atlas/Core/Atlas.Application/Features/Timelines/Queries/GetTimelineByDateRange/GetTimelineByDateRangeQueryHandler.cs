using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Timelines.Queries.GetTimelineByDateRange;

public class GetTimelineByDateRangeQueryHandler : IRequestHandler<GetTimelineByDateRangeQuery, ResponseModel<PagedResult>>
{
    public async Task<ResponseModel<PagedResult>> Handle(GetTimelineByDateRangeQuery request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}