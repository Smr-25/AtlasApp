using Atlas.Application.Common.Models;

namespace Atlas.Application.Common.Interfaces;

public interface IExternalAuthService
{
    Task<ExternalUserInfo?> ValidateAppleTokenAsync(string idToken);
    Task<ExternalUserInfo?> ValidateGoogleTokenAsync(string idToken);
}

 