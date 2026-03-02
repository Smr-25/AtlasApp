using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class PersonalAccessToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public string TokenHash { get; private set; } = null!;
    public string TokenPrefix { get; private set; } = null!;
    public string[] Scopes { get; private set; } = [];
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? LastUsedAt { get; private set; }
    public bool IsRevoked { get; private set; }

    private PersonalAccessToken() { }

    public static PersonalAccessToken Create(
        Guid userId, string name, string tokenHash, string tokenPrefix,
        string[] scopes, DateTime? expiresAt)
    {
        return new PersonalAccessToken
        {
            UserId = userId,
            Name = name,
            TokenHash = tokenHash,
            TokenPrefix = tokenPrefix,
            Scopes = scopes,
            ExpiresAt = expiresAt
        };
    }

    public void Revoke()
    {
        IsRevoked = true;
        SetModified();
    }

    public void RecordUsage()
    {
        LastUsedAt = DateTime.UtcNow;
    }

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    public bool IsValid => !IsRevoked && !IsExpired && !IsDeleted;
}

