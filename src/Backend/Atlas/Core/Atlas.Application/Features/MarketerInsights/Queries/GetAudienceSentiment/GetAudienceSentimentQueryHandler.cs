using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetAudienceSentiment;

public class GetAudienceSentimentQueryHandler(
    IMarketerInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetAudienceSentimentQuery, SentimentResult>
{
    public async Task<SentimentResult> Handle(GetAudienceSentimentQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await insightService.GetAudienceSentimentAsync(userId, request.From, request.To, cancellationToken);
    }
}

