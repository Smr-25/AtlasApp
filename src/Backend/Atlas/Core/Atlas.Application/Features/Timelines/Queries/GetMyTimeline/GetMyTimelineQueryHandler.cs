using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Timelines.Queries.GetMyTimeline;

public class GetMyTimelineQueryHandler : IRequestHandler<GetMyTimelineQuery, ResponseModel<PagedResult>>
{
    public async Task<ResponseModel<PagedResult>> Handle(GetMyTimelineQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}