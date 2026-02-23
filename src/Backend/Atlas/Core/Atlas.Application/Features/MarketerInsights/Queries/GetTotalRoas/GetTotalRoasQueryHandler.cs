using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetTotalRoas;

public class GetTotalRoasQueryHandler(
    IMarketerInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetTotalRoasQuery, TotalRoasResult>
{
    public async Task<TotalRoasResult> Handle(GetTotalRoasQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var roas = await insightService.GetTotalRoasAsync(userId, request.From, request.To, cancellationToken);
        return new TotalRoasResult(roas, 0, 0);
    }
}

