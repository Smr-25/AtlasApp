using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetMeetingsAvoided;

public class GetMeetingsAvoidedQueryHandler(
    ILeaderInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetMeetingsAvoidedQuery, MeetingsAvoidedResult>
{
    public async Task<MeetingsAvoidedResult> Handle(GetMeetingsAvoidedQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await insightService.GetMeetingsAvoidedAsync(userId, request.TeamId, request.From, request.To, cancellationToken);
    }
}

