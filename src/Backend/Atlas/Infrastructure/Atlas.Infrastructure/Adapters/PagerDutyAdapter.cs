using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class PagerDutyAdapter(IHttpClientFactory httpClientFactory) : IPagerDutyAdapter
{
    public Task<string> TriggerIncidentAsync(string serviceId, string title, string description, CancellationToken ct)
        => Task.FromResult($"Incident triggered: {title}");

    public Task<string> AcknowledgeIncidentAsync(string incidentId, CancellationToken ct)
        => Task.FromResult($"Incident {incidentId} acknowledged.");

    public Task<string> ResolveIncidentAsync(string incidentId, CancellationToken ct)
        => Task.FromResult($"Incident {incidentId} resolved.");

    public Task<List<PagerDutyIncident>> GetIncidentsAsync(string status, CancellationToken ct)
        => Task.FromResult(new List<PagerDutyIncident>());
}

