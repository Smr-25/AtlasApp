namespace Atlas.Application.Common.Interfaces;

public interface ILeaderScriptService
{
    Task<SprintStarterResult> RunSprintStarterAsync(Guid userId, string sprintName, List<string> initialTasks, Guid teamId, CancellationToken ct);
    Task<BlockedTaskBlasterResult> RunBlockedTaskBlasterAsync(Guid userId, Guid teamId, CancellationToken ct);
    Task<string> GenerateReleaseNotesAsync(Guid userId, string repoName, string fromTag, string toTag, CancellationToken ct);
    Task<string> ActivateMeetingModeAsync(Guid userId, int durationMinutes, CancellationToken ct);
    Task<WeekSummaryResult> GenerateWeekSummaryAsync(Guid userId, Guid teamId, CancellationToken ct);
    Task<BulkReassignResult> BulkReassignTasksAsync(Guid userId, Guid absentMemberId, Guid teamId, CancellationToken ct);
    Task<string> SendStandupPingAsync(Guid userId, Guid teamId, CancellationToken ct);
}

public record SprintStarterResult(string SprintId, string SprintName, int TasksCreated, string SlackNotification);
public record BlockedTaskBlasterResult(int BlockedTasksFound, List<BlockedTaskInfo> Tasks, int MessagesSent);
public record BlockedTaskInfo(string TaskKey, string Assignee, string Summary, int DaysBlocked);
public record WeekSummaryResult(int TasksCompleted, int BugsFixed, int PrsMerged, double VelocityPoints, string SummaryMarkdown);
public record BulkReassignResult(int TasksReassigned, List<ReassignedTaskInfo> Tasks);
public record ReassignedTaskInfo(string TaskKey, string FromUser, string ToUser);

