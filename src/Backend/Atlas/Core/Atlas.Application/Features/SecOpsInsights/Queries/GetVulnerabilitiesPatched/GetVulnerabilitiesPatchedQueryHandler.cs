using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetVulnerabilitiesPatched;

public class GetVulnerabilitiesPatchedQueryHandler(
    ISecOpsInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetVulnerabilitiesPatchedQuery, VulnerabilitiesPatchedResult>
{
    public async Task<VulnerabilitiesPatchedResult> Handle(GetVulnerabilitiesPatchedQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var total = await insightService.GetVulnerabilitiesPatchedAsync(userId, request.From, request.To, cancellationToken);
        return new VulnerabilitiesPatchedResult(total, 0, 0, 0, 0);
    }
}

