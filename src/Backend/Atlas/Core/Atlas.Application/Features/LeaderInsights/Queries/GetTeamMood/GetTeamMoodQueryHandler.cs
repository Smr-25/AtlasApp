using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetTeamMood;

public class GetTeamMoodQueryHandler(
    ILeaderInsightCalculationService insightService
) : IRequestHandler<GetTeamMoodQuery, TeamMoodResult>
{
    public async Task<TeamMoodResult> Handle(GetTeamMoodQuery request, CancellationToken cancellationToken)
    {
        return await insightService.GetTeamMoodAsync(request.TeamId, request.From, request.To, cancellationToken);
    }
}

