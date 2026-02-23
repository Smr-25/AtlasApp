using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class LeaderInsightCalculationService : ILeaderInsightCalculationService
{
    public Task<SprintVelocityResult> GetSprintVelocityAsync(Guid userId, Guid teamId, DateTime from, DateTime to, CancellationToken ct)
    {
        return Task.FromResult(new SprintVelocityResult(0, 0, []));
    }

    public Task<MeetingsAvoidedResult> GetMeetingsAvoidedAsync(Guid userId, Guid teamId, DateTime from, DateTime to, CancellationToken ct)
    {
        return Task.FromResult(new MeetingsAvoidedResult(0, 0, 0));
    }

    public Task<BlockedTimeResult> GetBlockedTimeAsync(Guid userId, Guid teamId, DateTime from, DateTime to, CancellationToken ct)
    {
        return Task.FromResult(new BlockedTimeResult(0, []));
    }

    public Task<CostPerFeatureResult> GetCostPerFeatureAsync(Guid userId, Guid teamId, DateTime from, DateTime to, CancellationToken ct)
    {
        return Task.FromResult(new CostPerFeatureResult([], 0));
    }

    public Task<ReviewTurnaroundResult> GetReviewTurnaroundAsync(Guid userId, Guid teamId, DateTime from, DateTime to, CancellationToken ct)
    {
        return Task.FromResult(new ReviewTurnaroundResult(0, 0, 0));
    }

    public Task<TopContributorResult> GetTopContributorAsync(Guid teamId, DateTime from, DateTime to, CancellationToken ct)
    {
        return Task.FromResult(new TopContributorResult("N/A", 0, 0, 0, 0));
    }

    public Task<TeamMoodResult> GetTeamMoodAsync(Guid teamId, DateTime from, DateTime to, CancellationToken ct)
    {
        return Task.FromResult(new TeamMoodResult(0, 0, "Neutral", []));
    }
}

