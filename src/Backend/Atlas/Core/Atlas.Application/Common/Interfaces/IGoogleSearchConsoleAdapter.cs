namespace Atlas.Application.Common.Interfaces;

public interface IGoogleSearchConsoleAdapter
{
    Task<List<SearchAnalyticsRow>> GetSearchAnalyticsAsync(string siteUrl, DateTime from, DateTime to, CancellationToken ct);
    Task<List<string>> GetSitemapsAsync(string siteUrl, CancellationToken ct);
}

public record SearchAnalyticsRow(string Query, double Clicks, double Impressions, double Ctr, double Position);

