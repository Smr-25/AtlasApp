namespace Atlas.Application.Common.Interfaces;

public interface ILeaderAgentService
{
    Task<BottleneckResult> PredictBottleneckAsync(Guid teamId, CancellationToken ct);
    Task<BurnoutRiskResult> DetectBurnoutRiskAsync(Guid teamId, CancellationToken ct);
    Task<ScopeCreepResult> DetectScopeCreepAsync(Guid teamId, string sprintId, CancellationToken ct);
    Task<PrReviewNagResult> NagPrReviewsAsync(Guid teamId, int thresholdHours, CancellationToken ct);
    Task<UnassignedBugResult> CatchUnassignedBugsAsync(Guid teamId, CancellationToken ct);
    Task<GhostMemberResult> PingGhostMembersAsync(Guid teamId, CancellationToken ct);
    Task<MilestoneCelebrationResult> CheckMilestoneCelebrationAsync(Guid teamId, CancellationToken ct);
}

public record BottleneckResult(List<BottleneckMember> Members);
public record BottleneckMember(string MemberName, string TaskKey, int DaysStuck, string Recommendation);
public record BurnoutRiskResult(List<BurnoutRiskMember> Members);
public record BurnoutRiskMember(string MemberName, double OvertimeHours, int LateNightCommits, string RiskLevel);
public record ScopeCreepResult(int OriginalTaskCount, int CurrentTaskCount, int AddedMidSprint, double CreepPercentage, string Warning);
public record PrReviewNagResult(List<StalePrInfo> StalePrs, int TotalStale);
public record StalePrInfo(string PrTitle, string Author, int HoursPending, string Url);
public record UnassignedBugResult(List<UnassignedBugInfo> Bugs, int TotalUnassigned);
public record UnassignedBugInfo(string IssueKey, string Title, string Severity, DateTime ReportedAt);
public record GhostMemberResult(List<GhostMemberInfo> GhostMembers);
public record GhostMemberInfo(string MemberName, DateTime LastActiveAt, int HoursInactive);
public record MilestoneCelebrationResult(bool HasMilestone, string? MilestoneName, double CompletionPercentage, string? CelebrationMessage);

