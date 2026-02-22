using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevInsights.Queries.GetTimeSaved;

public class GetTimeSavedQueryHandler(
    IInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetTimeSavedQuery, TimeSavedResult>
{
    public async Task<TimeSavedResult> Handle(GetTimeSavedQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var hours = await insightService.CalculateTimeSavedAsync(userId, request.From, request.To, cancellationToken);
        return new TimeSavedResult(hours, 0, 0);
    }
}

