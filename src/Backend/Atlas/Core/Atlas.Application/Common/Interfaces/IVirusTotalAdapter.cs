namespace Atlas.Application.Common.Interfaces;

public interface IVirusTotalAdapter
{
    Task<VirusTotalScanResult> ScanUrlAsync(string url, CancellationToken ct);
    Task<VirusTotalScanResult> ScanFileHashAsync(string fileHash, CancellationToken ct);
}

public record VirusTotalScanResult(string Resource, int Positives, int Total, string ScanDate, List<VirusTotalEngine> Engines);
public record VirusTotalEngine(string Name, bool Detected, string? Result);

