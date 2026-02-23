using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetOpenPortsGraph;

public class GetOpenPortsGraphQueryHandler(
    ISecOpsInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetOpenPortsGraphQuery, OpenPortsGraphResult>
{
    public async Task<OpenPortsGraphResult> Handle(GetOpenPortsGraphQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var data = await insightService.GetOpenPortsGraphAsync(userId, request.From, request.To, cancellationToken);
        return new OpenPortsGraphResult(data);
    }
}

