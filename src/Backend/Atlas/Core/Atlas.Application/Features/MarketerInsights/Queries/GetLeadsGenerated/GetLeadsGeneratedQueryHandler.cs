using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetLeadsGenerated;

public class GetLeadsGeneratedQueryHandler(
    IMarketerInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetLeadsGeneratedQuery, LeadsGeneratedResult>
{
    public async Task<LeadsGeneratedResult> Handle(GetLeadsGeneratedQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var total = await insightService.GetLeadsGeneratedAsync(userId, request.From, request.To, cancellationToken);
        return new LeadsGeneratedResult(total, 0, 0);
    }
}

