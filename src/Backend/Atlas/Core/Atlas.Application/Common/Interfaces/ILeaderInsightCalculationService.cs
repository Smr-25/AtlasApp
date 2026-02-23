namespace Atlas.Application.Common.Interfaces;

public interface ILeaderInsightCalculationService
{
    Task<SprintVelocityResult> GetSprintVelocityAsync(Guid userId, Guid teamId, DateTime from, DateTime to, CancellationToken ct);
    Task<MeetingsAvoidedResult> GetMeetingsAvoidedAsync(Guid userId, Guid teamId, DateTime from, DateTime to, CancellationToken ct);
    Task<BlockedTimeResult> GetBlockedTimeAsync(Guid userId, Guid teamId, DateTime from, DateTime to, CancellationToken ct);
    Task<CostPerFeatureResult> GetCostPerFeatureAsync(Guid userId, Guid teamId, DateTime from, DateTime to, CancellationToken ct);
    Task<ReviewTurnaroundResult> GetReviewTurnaroundAsync(Guid userId, Guid teamId, DateTime from, DateTime to, CancellationToken ct);
    Task<TopContributorResult> GetTopContributorAsync(Guid teamId, DateTime from, DateTime to, CancellationToken ct);
    Task<TeamMoodResult> GetTeamMoodAsync(Guid teamId, DateTime from, DateTime to, CancellationToken ct);
}

public record SprintVelocityResult(double TotalPoints, double AveragePerSprint, List<SprintVelocityPoint> DataPoints);
public record SprintVelocityPoint(string SprintName, double Points, DateTime EndDate);
public record MeetingsAvoidedResult(int MeetingsCancelled, double HoursSaved, double EstimatedMoneySaved);
public record BlockedTimeResult(double TotalBlockedHours, List<BlockedMemberTime> Members);
public record BlockedMemberTime(string MemberName, double BlockedHours, string TopBlocker);
public record CostPerFeatureResult(List<FeatureCostInfo> Features, double AverageCost);
public record FeatureCostInfo(string FeatureName, double EstimatedHours, double Cost);
public record ReviewTurnaroundResult(double AverageHours, double MedianHours, int TotalReviews);
public record TopContributorResult(string MemberName, int TasksClosed, int PrsMerged, int BugsFixed, int TotalScore);
public record TeamMoodResult(double StressLevel, double HappinessLevel, string OverallMood, List<MoodFactor> Factors);
public record MoodFactor(string Factor, double Impact, string Direction);

