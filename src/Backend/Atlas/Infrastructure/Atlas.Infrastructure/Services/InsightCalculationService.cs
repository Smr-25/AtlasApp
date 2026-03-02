using Atlas.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Services;

public class InsightCalculationService(
    IApplicationDbContext dbContext,
    ILogger<InsightCalculationService> logger
) : IInsightCalculationService
{
    public async Task<double> CalculateTimeSavedAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var scriptRuns = await dbContext.UserActivities
            .Where(a => a.UserId == userId && a.ActionType == "ScriptRun" && a.CreatedAt >= from && a.CreatedAt <= to)
            .CountAsync(ct);

        return scriptRuns * 0.1;
    }

    public async Task<Dictionary<string, double>> GetFocusHeatmapAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var sessions = await dbContext.FocusSessions
            .Where(s => s.UserId == userId && s.StartedAt >= from && s.StartedAt <= to && s.CompletedAt != null)
            .ToListAsync(ct);

        return sessions
            .GroupBy(s => s.StartedAt!.Value.DayOfWeek.ToString())
            .ToDictionary(g => g.Key, g => g.Sum(s => s.DurationMinutes) / 60.0);
    }

    public async Task<int> GetContextSwitchCountAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        return await dbContext.UserActivities
            .Where(a => a.UserId == userId && a.ActionType == "ContextSwitch" && a.CreatedAt >= from && a.CreatedAt <= to)
            .CountAsync(ct);
    }

    public async Task<int> CountTodoCommentsAsync(string projectPath, CancellationToken ct)
    {
        try
        {
            if (!Directory.Exists(projectPath)) return 0;

            var count = 0;
            var files = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var content = await File.ReadAllTextAsync(file, ct);
                count += content.Split("TODO", StringSplitOptions.None).Length - 1;
                count += content.Split("FIXME", StringSplitOptions.None).Length - 1;
                count += content.Split("HACK", StringSplitOptions.None).Length - 1;
            }
            return count;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to count TODO comments in {Path}", projectPath);
            return 0;
        }
    }

    public async Task<Dictionary<string, int>> GetCodeChurnAsync(Guid userId, Guid integrationId, CancellationToken ct)
    {
        await Task.CompletedTask;
        return new Dictionary<string, int>();
    }

    public async Task<double> GetDeploymentSuccessRateAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var snapshot = await dbContext.InsightSnapshots
            .Where(s => s.UserId == userId && s.MetricKey == "DeploymentSuccessRate")
            .OrderByDescending(s => s.RecordedAt)
            .FirstOrDefaultAsync(ct);

        return snapshot?.Value ?? 100.0;
    }

    public async Task<Dictionary<int, double>> GetPeakProductivityHoursAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var sessions = await dbContext.FocusSessions
            .Where(s => s.UserId == userId && s.StartedAt >= from && s.StartedAt <= to && s.CompletedAt != null)
            .ToListAsync(ct);

        return sessions
            .GroupBy(s => s.StartedAt!.Value.Hour)
            .ToDictionary(g => g.Key, g => g.Sum(s => s.DurationMinutes) / 60.0);
    }
}
