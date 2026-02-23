namespace Atlas.Application.Common.Interfaces;

public interface IAwsGuardDutyAdapter
{
    Task<List<GuardDutyFinding>> GetFindingsAsync(string detectorId, CancellationToken ct);
    Task<string> ArchiveFindingAsync(string detectorId, string findingId, CancellationToken ct);
}

public record GuardDutyFinding(string Id, string Type, string Severity, string Description, DateTime UpdatedAt);

