 using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DesignInsights.Queries.GetHandoffsCompleted;

public class GetHandoffsCompletedQueryHandler(
    IDesignInsightCalculationService designInsight,
    ICurrentUserService currentUser
) : IRequestHandler<GetHandoffsCompletedQuery, int>
{
    public async Task<int> Handle(GetHandoffsCompletedQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await designInsight.GetHandoffsCompletedAsync(userId, request.From, request.To, cancellationToken);
    }
}

