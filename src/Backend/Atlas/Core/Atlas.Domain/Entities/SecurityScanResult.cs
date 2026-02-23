using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class SecurityScanResult : BaseEntity
{
    public string ScanType { get; private set; } = null!;
    public string ResultJson { get; private set; } = null!;
    public double? Score { get; private set; }
    public DateTime ScannedAt { get; private set; }
    public Guid UserId { get; private set; }

    private SecurityScanResult() { }

    public static SecurityScanResult Create(
        Guid userId,
        string scanType,
        string resultJson,
        double? score = null)
    {
        return new SecurityScanResult
        {
            UserId = userId,
            ScanType = scanType,
            ResultJson = resultJson,
            Score = score,
            ScannedAt = DateTime.UtcNow
        };
    }
}

