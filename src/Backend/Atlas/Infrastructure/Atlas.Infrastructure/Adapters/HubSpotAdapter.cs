using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class HubSpotAdapter(IHttpClientFactory httpClientFactory) : IHubSpotAdapter
{
    public Task<string> CreateContactAsync(string email, string firstName, string lastName, CancellationToken ct)
        => Task.FromResult($"Contact created: {firstName} {lastName} ({email})");

    public Task<string> AssignContactToOwnerAsync(string contactId, string ownerId, CancellationToken ct)
        => Task.FromResult($"Contact {contactId} assigned to {ownerId}.");

    public Task<List<HubSpotContact>> GetRecentContactsAsync(int count, CancellationToken ct)
        => Task.FromResult(new List<HubSpotContact>());
}

