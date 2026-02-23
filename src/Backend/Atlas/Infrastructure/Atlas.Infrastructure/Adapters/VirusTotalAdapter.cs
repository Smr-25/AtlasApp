using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class VirusTotalAdapter(IHttpClientFactory httpClientFactory) : IVirusTotalAdapter
{
    public Task<VirusTotalScanResult> ScanUrlAsync(string url, CancellationToken ct)
        => Task.FromResult(new VirusTotalScanResult(url, 0, 70, DateTime.UtcNow.ToString("o"), []));

    public Task<VirusTotalScanResult> ScanFileHashAsync(string fileHash, CancellationToken ct)
        => Task.FromResult(new VirusTotalScanResult(fileHash, 0, 70, DateTime.UtcNow.ToString("o"), []));
}

