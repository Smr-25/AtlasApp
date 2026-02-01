using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public class Integration : BaseEntity
{
    public Guid PersonaId { get; private set; }
    public IntegrationProvider Provider { get; private set; }
    public string Name { get; private set; } = null!;
    public string? EncryptedAccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTimeOffset? TokenExpiresAt { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset? LastUsedAt { get; private set; }
    public string? Metadata { get; private set; }
    public Persona Persona { get; private set; } = null!;

    private Integration()
    {
    }

    public static Integration Create(
        Guid personaId,
        IntegrationProvider provider,
        string name,
        string? encryptedAccessToken = null,
        string? refreshToken = null,
        DateTimeOffset? tokenExpiresAt = null,
        string? metadata = null)
    {
        if (personaId == Guid.Empty)
            throw new InvalidEntityStateException(nameof(Integration), nameof(PersonaId),
                "Persona ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidEntityStateException(nameof(Integration), nameof(Name),
                "Integration name cannot be empty.");

        if (name.Length > 100)
            throw new InvalidEntityStateException(nameof(Integration), nameof(Name),
                "Integration name cannot exceed 100 characters.");

        return new Integration
        {
            PersonaId = personaId,
            Provider = provider,
            Name = name.Trim(),
            EncryptedAccessToken = encryptedAccessToken,
            RefreshToken = refreshToken,
            TokenExpiresAt = tokenExpiresAt,
            Metadata = metadata,
            IsActive = true
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidEntityStateException(nameof(Integration), nameof(Name),
                "Integration name cannot be empty.");

        if (name.Length > 100)
            throw new InvalidEntityStateException(nameof(Integration), nameof(Name),
                "Integration name cannot exceed 100 characters.");

        Name = name.Trim();
        SetModified();
    }

    public void UpdateTokens(
        string encryptedAccessToken,
        string? refreshToken = null,
        DateTimeOffset? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(encryptedAccessToken))
            throw new InvalidEntityStateException(nameof(Integration), nameof(EncryptedAccessToken),
                "Access token cannot be empty.");

        EncryptedAccessToken = encryptedAccessToken;

        if (refreshToken != null)
            RefreshToken = refreshToken;

        TokenExpiresAt = expiresAt;
        SetModified();
    }

    public void RotateToken(
        string newEncryptedAccessToken,
        string? newRefreshToken = null,
        DateTimeOffset? newExpiresAt = null)
    {
        UpdateTokens(newEncryptedAccessToken, newRefreshToken, newExpiresAt);
    }

    public void UpdateMetadata(string? metadata)
    {
        Metadata = metadata;
        SetModified();
    }

    public void RecordUsage()
    {
        LastUsedAt = DateTimeOffset.UtcNow;
        SetModified();
    }

    public void Activate()
    {
        if (IsActive) return;

        IsActive = true;
        SetModified();
    }

    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        SetModified();
    }

    public void Revoke()
    {
        EncryptedAccessToken = null;
        RefreshToken = null;
        TokenExpiresAt = null;
        IsActive = false;
        SetModified();
    }

    public bool IsTokenExpired(int bufferMinutes = 5)
    {
        if (!TokenExpiresAt.HasValue)
            return false;

        return TokenExpiresAt.Value <= DateTimeOffset.UtcNow.AddMinutes(bufferMinutes);
    }

    public bool IsUsable()
    {
        return IsActive &&
               !IsDeleted &&
               !string.IsNullOrEmpty(EncryptedAccessToken) &&
               !IsTokenExpired();
    }
}