using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetAverageResponseTime;

public class GetAverageResponseTimeQueryHandler(
    ISecOpsInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetAverageResponseTimeQuery, AverageResponseTimeResult>
{
    public async Task<AverageResponseTimeResult> Handle(GetAverageResponseTimeQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var avg = await insightService.GetAverageResponseTimeAsync(userId, request.From, request.To, cancellationToken);
        return new AverageResponseTimeResult(avg, 0, 0);
    }
}

