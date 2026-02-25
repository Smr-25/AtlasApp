using Atlas.Application.Common.Models;

namespace Atlas.Application.Common.Interfaces;

public interface IExternalAuthService
{
    Task<ExternalUserInfo?> ValidateGoogleTokenAsync(string idToken);
    Task<ExternalUserInfo?> ValidateGitHubTokenAsync(string? accessToken, string? authorizationCode = null);
}
