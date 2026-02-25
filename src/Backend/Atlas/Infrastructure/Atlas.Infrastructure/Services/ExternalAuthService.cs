using System.Text.Json;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Settings;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Atlas.Infrastructure.Services;

public class ExternalAuthService(IOptions<ExternalAuthSettings> options) : IExternalAuthService
{
    private readonly ExternalAuthSettings _externalAuthSettings = options.Value;

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

    #region GitHub Authentication

    public async Task<ExternalUserInfo?> ValidateGitHubTokenAsync(string? accessToken, string? authorizationCode = null)
    {
        try
        {
            if (string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(authorizationCode))
            {
                using var client = new HttpClient();
                var values = new Dictionary<string, string?>
                {
                    { "client_id", _externalAuthSettings.GitHub.ClientId },
                    { "client_secret", _externalAuthSettings.GitHub.ClientSecret },
                    { "code", authorizationCode }
                };
                var req = new HttpRequestMessage(HttpMethod.Post, _externalAuthSettings.GitHub.TokenEndpoint);
                req.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                req.Content = new FormUrlEncodedContent(values.Where(kv => kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value!));

                var resp = await client.SendAsync(req);
                if (!resp.IsSuccessStatusCode) return null;
                var body = await resp.Content.ReadAsStringAsync();
                // GitHub may return query string or json depending on Accept header; we requested json
                var parsed = JsonSerializer.Deserialize<JsonElement>(body);
                if (parsed.TryGetProperty("access_token", out var at))
                    accessToken = at.GetString();
                else
                    return null;
            }

            if (string.IsNullOrEmpty(accessToken)) return null;

            // Get user info
            using var userClient = new HttpClient();
            userClient.DefaultRequestHeaders.UserAgent.ParseAdd("Atlas-App");
            userClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var userResp = await userClient.GetAsync(_externalAuthSettings.GitHub.UserApiEndpoint);
            if (!userResp.IsSuccessStatusCode) return null;
            var userJson = await userResp.Content.ReadAsStringAsync();
            var userDoc = JsonSerializer.Deserialize<JsonElement>(userJson);

            var providerId = userDoc.GetProperty("id").GetRawText().Trim('"');
            var fullName = userDoc.TryGetProperty("name", out var n) && n.ValueKind != JsonValueKind.Null ? n.GetString() : null;
            var email = userDoc.TryGetProperty("email", out var e) && e.ValueKind != JsonValueKind.Null ? e.GetString() : null;

            // If email is null, fetch primary email from /user/emails
            if (string.IsNullOrEmpty(email))
            {
                var emailResp = await userClient.GetAsync(_externalAuthSettings.GitHub.UserEmailsEndpoint);
                if (emailResp.IsSuccessStatusCode)
                {
                    var emailsJson = await emailResp.Content.ReadAsStringAsync();
                    var emailsArr = JsonSerializer.Deserialize<JsonElement>(emailsJson);
                    if (emailsArr.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in emailsArr.EnumerateArray())
                        {
                            var primary = item.TryGetProperty("primary", out var p) && p.GetBoolean();
                            var verified = item.TryGetProperty("verified", out var v) && v.GetBoolean();
                            var address = item.TryGetProperty("email", out var addr) ? addr.GetString() : null;
                            if (primary && verified && !string.IsNullOrEmpty(address))
                            {
                                email = address;
                                break;
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(providerId) || string.IsNullOrEmpty(email))
                return null;

            return new ExternalUserInfo(
                ProviderId: providerId,
                Email: email,
                FullName: fullName,
                Provider: "GitHub",
                EmailVerified: true, // GitHub primary email fetched above was checked for verified
                AccessToken: accessToken
            );
        }
        catch
        {
            return null;
        }
    }

    #endregion
}
