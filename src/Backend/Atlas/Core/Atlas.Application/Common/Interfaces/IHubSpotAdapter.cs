namespace Atlas.Application.Common.Interfaces;

public interface IHubSpotAdapter
{
    Task<string> CreateContactAsync(string email, string firstName, string lastName, CancellationToken ct);
    Task<string> AssignContactToOwnerAsync(string contactId, string ownerId, CancellationToken ct);
    Task<List<HubSpotContact>> GetRecentContactsAsync(int count, CancellationToken ct);
}

public record HubSpotContact(string Id, string Email, string FirstName, string LastName, DateTime CreatedAt);

