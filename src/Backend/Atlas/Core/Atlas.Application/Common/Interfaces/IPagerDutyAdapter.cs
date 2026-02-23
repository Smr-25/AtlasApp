namespace Atlas.Application.Common.Interfaces;

public interface IPagerDutyAdapter
{
    Task<string> TriggerIncidentAsync(string serviceId, string title, string description, CancellationToken ct);
    Task<string> AcknowledgeIncidentAsync(string incidentId, CancellationToken ct);
    Task<string> ResolveIncidentAsync(string incidentId, CancellationToken ct);
    Task<List<PagerDutyIncident>> GetIncidentsAsync(string status, CancellationToken ct);
}

public record PagerDutyIncident(string Id, string Title, string Status, string Urgency, DateTime CreatedAt);

