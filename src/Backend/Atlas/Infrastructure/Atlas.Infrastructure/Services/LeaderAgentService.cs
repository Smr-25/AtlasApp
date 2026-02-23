using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class LeaderAgentService(IJiraAdapter jiraAdapter, IGitIntegrationAdapter gitAdapter) : ILeaderAgentService
{
    public Task<BottleneckResult> PredictBottleneckAsync(Guid teamId, CancellationToken ct)
    {
        return Task.FromResult(new BottleneckResult([]));
    }

    public Task<BurnoutRiskResult> DetectBurnoutRiskAsync(Guid teamId, CancellationToken ct)
    {
        return Task.FromResult(new BurnoutRiskResult([]));
    }

    public Task<ScopeCreepResult> DetectScopeCreepAsync(Guid teamId, string sprintId, CancellationToken ct)
    {
        return Task.FromResult(new ScopeCreepResult(0, 0, 0, 0, "No sprint data available."));
    }

    public Task<PrReviewNagResult> NagPrReviewsAsync(Guid teamId, int thresholdHours, CancellationToken ct)
    {
        return Task.FromResult(new PrReviewNagResult([], 0));
    }

    public Task<UnassignedBugResult> CatchUnassignedBugsAsync(Guid teamId, CancellationToken ct)
    {
        return Task.FromResult(new UnassignedBugResult([], 0));
    }

    public Task<GhostMemberResult> PingGhostMembersAsync(Guid teamId, CancellationToken ct)
    {
        return Task.FromResult(new GhostMemberResult([]));
    }

    public Task<MilestoneCelebrationResult> CheckMilestoneCelebrationAsync(Guid teamId, CancellationToken ct)
    {
        return Task.FromResult(new MilestoneCelebrationResult(false, null, 0, null));
    }
}

