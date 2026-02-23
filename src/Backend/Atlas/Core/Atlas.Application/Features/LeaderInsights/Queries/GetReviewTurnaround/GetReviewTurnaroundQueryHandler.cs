using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetReviewTurnaround;

public class GetReviewTurnaroundQueryHandler(
    ILeaderInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetReviewTurnaroundQuery, ReviewTurnaroundResult>
{
    public async Task<ReviewTurnaroundResult> Handle(GetReviewTurnaroundQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await insightService.GetReviewTurnaroundAsync(userId, request.TeamId, request.From, request.To, cancellationToken);
    }
}

