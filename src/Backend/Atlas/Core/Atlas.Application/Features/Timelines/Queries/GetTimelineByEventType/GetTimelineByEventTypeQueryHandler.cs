using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Timelines.Queries.GetTimelineByEventType;

public class GetTimelineByEventTypeQueryHandler : IRequestHandler<GetTimelineByEventTypeQuery, ResponseModel<PagedResult>>
{
    public async Task<ResponseModel<PagedResult>> Handle(GetTimelineByEventTypeQuery request, CancellationToken cancellationToken)
    {
        throw  new NotImplementedException();
    }
}