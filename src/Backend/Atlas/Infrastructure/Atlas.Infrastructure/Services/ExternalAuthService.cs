using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Settings;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Atlas.Infrastructure.Services;

public class ExternalAuthService(IOptions<ExternalAuthSettings> options) : IExternalAuthService
{
    private readonly ExternalAuthSettings _externalAuthSettings = options.Value;

    #region Apple Authentication

    public async Task<ExternalUserInfo?> ValidateAppleTokenAsync(string idToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var applePublicKeys = await GetApplePublicKeysAsync();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "https://appleid.apple.com",
                ValidateAudience = true,
                ValidAudience = _externalAuthSettings.Apple.ClientId,
                ValidateLifetime = true,
                IssuerSigningKeys = applePublicKeys
            };
            var principal = tokenHandler.ValidateToken(idToken, validationParameters, out _);

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? principal.FindFirst("sub")?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value
                        ?? principal.FindFirst("email")?.Value;
            var emailVerified = principal.FindFirst("email_verified")?.Value == "true";
            var fullName = principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                return null;

            return new ExternalUserInfo(
                ProviderId: userId,
                Email: email,
                FullName: fullName,
                Provider: "Apple",
                EmailVerified: emailVerified
            );
        }
        catch
        {
            return null;
        }
    }

    private static async Task<IEnumerable<SecurityKey>> GetApplePublicKeysAsync()
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetStringAsync("https://appleid.apple.com/auth/keys");
        var jwks = new JsonWebKeySet(response);
        return jwks.GetSigningKeys();
    }

    #endregion

    #region Google Authentication

    public async Task<ExternalUserInfo?> ValidateGoogleTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = [_externalAuthSettings.Google.ClientId]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new ExternalUserInfo(
                ProviderId: payload.Subject,
                Email: payload.Email,
                FullName: payload.Name,
                Provider: "Google",
                EmailVerified: payload.EmailVerified
            );
        }
        catch
        {
            return null;
        }
    }

    #endregion
}
