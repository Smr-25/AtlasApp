namespace Atlas.Application.Features.PersonalTokens.Dtos;

public record PersonalTokenDto(
    Guid Id,
    string Name,
    string TokenPrefix,
    string[] Scopes,
    DateTime? ExpiresAt,
    DateTime? LastUsedAt,
    bool IsRevoked,
    DateTimeOffset CreatedAt
);

public record CreatedTokenDto(
    Guid Id,
    string Name,
    string Token,
    string[] Scopes,
    DateTime? ExpiresAt
);

