namespace Atlas.Application.Common.Interfaces;

public interface IGA4Adapter
{
    Task<GA4RealtimeResult> GetRealtimeUsersAsync(string propertyId, CancellationToken ct);
    Task<List<GA4PageView>> GetTopPagesAsync(string propertyId, DateTime from, DateTime to, CancellationToken ct);
}

public record GA4RealtimeResult(int ActiveUsers, List<GA4ActivePage> TopPages);
public record GA4ActivePage(string PagePath, int ActiveUsers);
public record GA4PageView(string PagePath, int Views, double AvgDuration);

