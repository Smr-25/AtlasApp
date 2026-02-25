namespace Atlas.Application.Common.Models;

public record ExternalUserInfo(
    string ProviderId,
    string Email,
    string? FullName,
    string Provider,
    bool EmailVerified,
    string? AccessToken = null
);