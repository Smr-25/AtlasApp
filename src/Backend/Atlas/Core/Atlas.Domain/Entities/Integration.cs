using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class Integration : BaseEntity
{
    public string Name { get; private set; } = null!; 
    public IntegrationProvider Provider { get; private set; }
    public IntegrationStatus Status { get; private set; }
    public string EncryptedAccessToken { get; private set; } = null!;
    public string? EncryptedRefreshToken { get; private set; }
    public DateTime? TokenExpiresAt { get; private set; }
    public string? MetadataJson { get; private set; }
    public Guid UserProfileId { get; private set; }

    private readonly List<WorkspaceIntegration> _workspaceConnections = [];
    public IReadOnlyCollection<WorkspaceIntegration> WorkspaceConnections => _workspaceConnections.AsReadOnly();

    private Integration() { }

    public static Integration Create(
        Guid userProfileId, 
        string name, 
        IntegrationProvider provider, 
        string encryptedAccessToken, 
        string? encryptedRefreshToken,
        DateTime? expiresAt,
        string? metadataJson)
    {
        return new Integration
        {
            UserProfileId = userProfileId,
            Name = name,
            Provider = provider,
            EncryptedAccessToken = encryptedAccessToken,
            EncryptedRefreshToken = encryptedRefreshToken,
            TokenExpiresAt = expiresAt,
            MetadataJson = metadataJson,
            Status = IntegrationStatus.Active
        };
    }

    public void UpdateTokens(string encryptedAccessToken, string? encryptedRefreshToken, DateTime? expiresAt)
    {
        EncryptedAccessToken = encryptedAccessToken;
        EncryptedRefreshToken = encryptedRefreshToken;
        TokenExpiresAt = expiresAt;
        Status = IntegrationStatus.Active; 
        SetModified();
    }

    public void MarkAsExpired()
    {
        Status = IntegrationStatus.Expired;
        SetModified();
    }
    
    public void Rename(string name)
    {
        Name = name;
        SetModified();
    }

    public void Delete()
    {
        Status = IntegrationStatus.Disconnected;
        SetDelete();
        SetModified();
    }

    public static Integration CreatePlaceholder(Guid userId, IntegrationProvider provider, string name)
    {
        return new Integration
        {
            UserProfileId = userId,
            Name = name,
            Provider = provider,
            Status = IntegrationStatus.PendingSetup
        };
    }

    public void UpdateMetadata(string metadataJson)
    {
        MetadataJson = metadataJson;
        SetModified();
    }
}