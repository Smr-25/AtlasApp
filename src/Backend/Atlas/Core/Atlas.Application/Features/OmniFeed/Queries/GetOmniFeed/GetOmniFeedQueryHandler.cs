using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.OmniFeed.Queries.GetOmniFeed;

public class GetOmniFeedQueryHandler(
    IOmniFeedService feedService
) : IRequestHandler<GetOmniFeedQuery, OmniFeedPage>
{
    public async Task<OmniFeedPage> Handle(GetOmniFeedQuery request, CancellationToken cancellationToken)
    {
        return await feedService.GetFeedAsync(request.TeamId, request.SourceFilter, request.Page, request.PageSize, cancellationToken);
    }
}

