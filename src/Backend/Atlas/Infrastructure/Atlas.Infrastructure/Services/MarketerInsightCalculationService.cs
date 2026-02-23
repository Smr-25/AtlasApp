using Atlas.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Services;

public class MarketerInsightCalculationService(IApplicationDbContext dbContext) : IMarketerInsightCalculationService
{
    public async Task<double> GetTotalRoasAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var metrics = await dbContext.MarketingCampaignMetrics
            .Where(m => m.UserId == userId && m.RecordedAt >= from && m.RecordedAt <= to)
            .ToListAsync(ct);

        var totalSpend = metrics.Sum(m => m.Spend);
        return totalSpend > 0 ? metrics.Sum(m => m.Revenue) / totalSpend : 0;
    }

    public async Task<int> GetLeadsGeneratedAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        return await dbContext.MarketerInsightSnapshots
            .Where(s => s.UserId == userId && s.RecordedAt >= from && s.RecordedAt <= to &&
                        s.MetricKey == "LeadsGenerated")
            .SumAsync(s => (int)s.Value, ct);
    }

    public async Task<int> GetZombieAdsKilledAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        return await dbContext.MarketerAlerts
            .Where(a => a.UserId == userId && a.CreatedAt >= from && a.CreatedAt <= to && a.IsActioned)
            .CountAsync(ct);
    }

    public async Task<double> GetAbTestWinRateAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var snapshots = await dbContext.MarketerInsightSnapshots
            .Where(s => s.UserId == userId && s.RecordedAt >= from && s.RecordedAt <= to &&
                        s.MetricKey == "AbTestWinRate")
            .ToListAsync(ct);

        return snapshots.Count > 0 ? snapshots.Average(s => s.Value) : 0;
    }

    public async Task<Dictionary<int, double>> GetPeakEngagementHoursAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var data = await dbContext.MarketerInsightSnapshots
            .Where(s => s.UserId == userId && s.RecordedAt >= from && s.RecordedAt <= to &&
                        s.MetricKey == "Engagement")
            .GroupBy(s => s.RecordedAt.Hour)
            .Select(g => new { Hour = g.Key, Value = g.Sum(x => x.Value) })
            .ToDictionaryAsync(g => g.Hour, g => g.Value, ct);
        return data;
    }

    public async Task<SentimentResult> GetAudienceSentimentAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var snapshots = await dbContext.MarketerInsightSnapshots
            .Where(s => s.UserId == userId && s.RecordedAt >= from && s.RecordedAt <= to &&
                        s.MetricKey == "Sentiment")
            .ToListAsync(ct);

        if (snapshots.Count == 0)
            return new SentimentResult(0, 0, 0, 0);

        return new SentimentResult(60, 20, 20, snapshots.Count);
    }

    public async Task<double> GetTimeSavedOnReportingAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var count = await dbContext.MarketerInsightSnapshots
            .Where(s => s.UserId == userId && s.RecordedAt >= from && s.RecordedAt <= to &&
                        s.MetricKey == "ReportGenerated")
            .CountAsync(ct);
        return count * 0.5;
    }
}

