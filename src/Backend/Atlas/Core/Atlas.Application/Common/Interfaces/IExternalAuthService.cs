using Atlas.Application.Models;

namespace Atlas.Application.Services.Interfaces;

public interface IExternalAuthService
{
    Task<ExternalUserInfo?> ValidateAppleTokenAsync(string idToken);
    Task<ExternalUserInfo?> ValidateGoogleTokenAsync(string idToken);
}

 