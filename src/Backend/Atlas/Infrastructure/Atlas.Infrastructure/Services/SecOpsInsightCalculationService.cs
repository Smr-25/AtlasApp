using Atlas.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Services;

public class SecOpsInsightCalculationService(IApplicationDbContext dbContext) : ISecOpsInsightCalculationService
{
    public async Task<int> GetThreatsBlockedAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        return await dbContext.SecOpsAlerts
            .Where(a => a.UserId == userId && a.CreatedAt >= from && a.CreatedAt <= to && a.IsActioned)
            .CountAsync(ct);
    }

    public async Task<int> GetVulnerabilitiesPatchedAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        return await dbContext.SecurityScanResults
            .Where(s => s.UserId == userId && s.ScannedAt >= from && s.ScannedAt <= to)
            .CountAsync(ct);
    }

    public async Task<double> GetAverageResponseTimeAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var alerts = await dbContext.SecOpsAlerts
            .Where(a => a.UserId == userId && a.CreatedAt >= from && a.CreatedAt <= to && a.IsActioned && a.ModifiedAt != null)
            .Select(a => (a.ModifiedAt!.Value - a.CreatedAt).TotalMinutes)
            .ToListAsync(ct);

        return alerts.Count > 0 ? alerts.Average() : 0;
    }

    public async Task<double> GetSecurityScoreAsync(Guid userId, CancellationToken ct)
    {
        var totalAlerts = await dbContext.SecOpsAlerts.Where(a => a.UserId == userId).CountAsync(ct);
        var resolvedAlerts = await dbContext.SecOpsAlerts.Where(a => a.UserId == userId && a.IsActioned).CountAsync(ct);

        if (totalAlerts == 0) return 100;
        return Math.Round((double)resolvedAlerts / totalAlerts * 100, 2);
    }

    public async Task<int> GetZeroIncidentStreakAsync(Guid userId, CancellationToken ct)
    {
        var lastIncident = await dbContext.SecOpsAlerts
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (lastIncident == null) return (DateTime.UtcNow - new DateTime(2025, 1, 1)).Days;
        return (DateTimeOffset.UtcNow - lastIncident.CreatedAt).Days;
    }

    public async Task<long> GetScannedBytesAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var snapshots = await dbContext.SecOpsInsightSnapshots
            .Where(s => s.UserId == userId && s.RecordedAt >= from && s.RecordedAt <= to &&
                        s.MetricKey == "ScannedBytes")
            .SumAsync(s => (long)s.Value, ct);
        return snapshots;
    }

    public async Task<Dictionary<DateTime, int>> GetOpenPortsGraphAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var data = await dbContext.SecOpsInsightSnapshots
            .Where(s => s.UserId == userId && s.RecordedAt >= from && s.RecordedAt <= to &&
                        s.MetricKey == "OpenPorts")
            .GroupBy(s => s.RecordedAt.Date)
            .Select(g => new { Date = g.Key, Count = (int)g.Sum(x => x.Value) })
            .ToDictionaryAsync(g => g.Date, g => g.Count, ct);
        return data;
    }
}

