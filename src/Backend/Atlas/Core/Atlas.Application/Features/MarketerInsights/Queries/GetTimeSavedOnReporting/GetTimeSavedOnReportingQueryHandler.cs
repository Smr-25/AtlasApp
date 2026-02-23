using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetTimeSavedOnReporting;

public class GetTimeSavedOnReportingQueryHandler(
    IMarketerInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetTimeSavedOnReportingQuery, TimeSavedOnReportingResult>
{
    public async Task<TimeSavedOnReportingResult> Handle(GetTimeSavedOnReportingQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var hours = await insightService.GetTimeSavedOnReportingAsync(userId, request.From, request.To, cancellationToken);
        return new TimeSavedOnReportingResult(hours, 0);
    }
}

