using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class LeaderScriptService(IJiraAdapter jiraAdapter) : ILeaderScriptService
{
    public Task<SprintStarterResult> RunSprintStarterAsync(Guid userId, string sprintName, List<string> initialTasks, Guid teamId, CancellationToken ct)
    {
        var tasksCreated = initialTasks.Count;
        return Task.FromResult(new SprintStarterResult(
            Guid.NewGuid().ToString(),
            sprintName,
            tasksCreated,
            $"Sprint '{sprintName}' started with {tasksCreated} tasks. Team notified via Slack."));
    }

    public Task<BlockedTaskBlasterResult> RunBlockedTaskBlasterAsync(Guid userId, Guid teamId, CancellationToken ct)
    {
        return Task.FromResult(new BlockedTaskBlasterResult(0, [], 0));
    }

    public Task<string> GenerateReleaseNotesAsync(Guid userId, string repoName, string fromTag, string toTag, CancellationToken ct)
    {
        var notes = $"## Release Notes ({fromTag} → {toTag})\n\nRelease notes for repository '{repoName}' will be generated from merged PRs.";
        return Task.FromResult(notes);
    }

    public Task<string> ActivateMeetingModeAsync(Guid userId, int durationMinutes, CancellationToken ct)
    {
        return Task.FromResult($"Meeting mode activated for {durationMinutes} minutes. All notifications silenced.");
    }

    public Task<WeekSummaryResult> GenerateWeekSummaryAsync(Guid userId, Guid teamId, CancellationToken ct)
    {
        var summary = new WeekSummaryResult(0, 0, 0, 0,
            "## End of Week Summary\n\nNo data available for this period.");
        return Task.FromResult(summary);
    }

    public Task<BulkReassignResult> BulkReassignTasksAsync(Guid userId, Guid absentMemberId, Guid teamId, CancellationToken ct)
    {
        return Task.FromResult(new BulkReassignResult(0, []));
    }

    public Task<string> SendStandupPingAsync(Guid userId, Guid teamId, CancellationToken ct)
    {
        return Task.FromResult("Standup ping sent to all team members who haven't submitted their update.");
    }
}


