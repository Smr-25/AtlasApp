using Atlas.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Services;

public class DesignInsightCalculationService(
    IApplicationDbContext dbContext,
    ILogger<DesignInsightCalculationService> logger
) : IDesignInsightCalculationService
{
    public async Task<double> GetAssetsOptimizedSavingsAsync(Guid userId, CancellationToken ct)
    {
        var assets = await dbContext.DesignAssets
            .Where(a => a.UserId == userId && a.IsOptimized)
            .ToListAsync(ct);

        var totalSaved = assets.Sum(a => a.OriginalSizeBytes - a.OptimizedSizeBytes);
        return totalSaved / (1024.0 * 1024.0);
    }

    public async Task<int> GetHandoffsCompletedAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        return await dbContext.DesignHandoffs
            .Where(h => h.DesignerId == userId && h.Status == "Completed" && h.CreatedAt >= from && h.CreatedAt <= to)
            .CountAsync(ct);
    }

    public async Task<double> GetFigmaFocusTimeAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var sessions = await dbContext.FocusSessions
            .Where(s => s.UserId == userId && s.Tag.Contains("Figma") && s.StartedAt >= from && s.StartedAt <= to)
            .ToListAsync(ct);

        return sessions.Sum(s => s.DurationMinutes) / 60.0;
    }

    public async Task<Dictionary<string, int>> GetColorTrendAsync(Guid userId, CancellationToken ct)
    {
        var colors = await dbContext.PaletteColors
            .Join(dbContext.DesignPalettes, c => c.PaletteId, p => p.Id, (c, p) => new { c.HexCode, p.UserId })
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.HexCode)
            .Select(g => new { Color = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync(ct);

        return colors.ToDictionary(c => c.Color, c => c.Count);
    }

    public async Task<double> GetReusabilityScoreAsync(Guid userId, CancellationToken ct)
    {
        var totalAssets = await dbContext.DesignAssets.Where(a => a.UserId == userId).CountAsync(ct);
        if (totalAssets == 0) return 100.0;

        var reusedAssets = await dbContext.DesignAssets
            .Where(a => a.UserId == userId)
            .GroupBy(a => a.OriginalFileName)
            .Where(g => g.Count() > 1)
            .CountAsync(ct);

        return Math.Round((double)reusedAssets / totalAssets * 100, 2);
    }

    public async Task<double> GetBrowserFreeHoursAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct)
    {
        var totalFocusMinutes = await dbContext.FocusSessions
            .Where(s => s.UserId == userId && s.StartedAt >= from && s.StartedAt <= to && s.CompletedAt != null)
            .SumAsync(s => s.DurationMinutes, ct);

        return totalFocusMinutes / 60.0;
    }

    public async Task<int> GetDesignDebtCountAsync(Guid userId, CancellationToken ct)
    {
        var nonOptimized = await dbContext.DesignAssets
            .Where(a => a.UserId == userId && !a.IsOptimized)
            .CountAsync(ct);

        return nonOptimized;
    }
}

