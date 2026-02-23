using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class AwsGuardDutyAdapter(IHttpClientFactory httpClientFactory) : IAwsGuardDutyAdapter
{
    public Task<List<GuardDutyFinding>> GetFindingsAsync(string detectorId, CancellationToken ct)
        => Task.FromResult(new List<GuardDutyFinding>());

    public Task<string> ArchiveFindingAsync(string detectorId, string findingId, CancellationToken ct)
        => Task.FromResult($"Finding {findingId} archived.");
}

