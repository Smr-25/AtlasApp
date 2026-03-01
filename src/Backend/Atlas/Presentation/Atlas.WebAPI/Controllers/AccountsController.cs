using Atlas.Application.Features.Accounts.Commands.AddPhoneNumber;
using Atlas.Application.Features.Accounts.Commands.ChangePassword;
using Atlas.Application.Features.Accounts.Commands.DeleteAccount;
using Atlas.Application.Features.Accounts.Commands.ExternalLogin;
using Atlas.Application.Features.Accounts.Commands.ForgotPassword;
using Atlas.Application.Features.Accounts.Commands.GenerateTelegramLinkCode;
using Atlas.Application.Features.Accounts.Commands.Login;
using Atlas.Application.Features.Accounts.Commands.Logout;
using Atlas.Application.Features.Accounts.Commands.RefreshToken;
using Atlas.Application.Features.Accounts.Commands.Register;
using Atlas.Application.Features.Accounts.Commands.ResendEmailVerification;
using Atlas.Application.Features.Accounts.Commands.ResendPhoneVerification;
using Atlas.Application.Features.Accounts.Commands.ResetPassword;
using Atlas.Application.Features.Accounts.Commands.RevokeToken;
using Atlas.Application.Features.Accounts.Commands.SetTelegramChatId;
using Atlas.Application.Features.Accounts.Commands.UpdateProfile;
using Atlas.Application.Features.Accounts.Commands.VerifyEmail;
using Atlas.Application.Features.Accounts.Commands.VerifyPhone;
using Atlas.Application.Features.Accounts.Commands.VerifyResetCode;
using Atlas.Application.Features.Accounts.Queries.GetProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Atlas.Application.Settings;
using System.Text.Json; 

namespace Atlas.WebAPI.Controllers;

[EnableRateLimiting("fixed")]
public class AccountsController : ApiControllerBase
{
    #region Auth Endpoints (Public)
    
    [HttpPost("register")]
    [EnableRateLimiting("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("external-login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        await Mediator.Send(command);
        return NoContentResponse();
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("verify-reset-code")]
    [EnableRateLimiting("verification")]
    public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("verify-email")]
    [EnableRateLimiting("verification")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("verify-phone")]
    [EnableRateLimiting("verification")]
    public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("resend-email-verification-code")]
    [EnableRateLimiting("resend")]
    public async Task<IActionResult> ResendEmailVerificationCode([FromBody] ResendEmailVerificationCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("resend-phone-verification-code")]
    [EnableRateLimiting("resend")]
    public async Task<IActionResult> ResendPhoneVerificationCode([FromBody] ResendPhoneVerificationCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeAllTokenCommand command)
    {
        await Mediator.Send(command);
        return NoContentResponse();
    }
    
    #endregion
    
    #region Profile Endpoints (Authorized)

    [Authorize]
    [HttpGet("profile")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await Mediator.Send(new GetProfileQuery());
        return OkResponse(result);
    }
    
    [Authorize]
    [HttpPut("profile")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
    
    [Authorize]
    [HttpPut("change-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
    
    [Authorize]
    [HttpPost("add-phone-number")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> AddPhoneNumber([FromBody] AddPhoneNumberCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
    
    [Authorize]
    [HttpDelete("delete-account")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> DeleteAccount()
    {
        var result = await Mediator.Send(new DeleteAccountCommand());
        return OkResponse(result);
    }
    
    [Authorize]
    [HttpPost("set-telegram-chat-id")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> SetTelegramChatId([FromBody] SetTelegramChatIdCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
    
    [Authorize]
    [HttpPost("generate-telegram-link-code")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> GenerateTelegramLinkCode()
    {
        var result = await Mediator.Send(new GenerateTelegramLinkCodeCommand());
        return OkResponse(result);
    }
    
    #endregion

    [HttpGet("external/{provider}")]
    [AllowAnonymous]
    public IActionResult ExternalRedirect(string provider)
    {
        if (string.IsNullOrEmpty(provider)) return BadRequestResponse("Provider is required");
        provider = provider.ToLower();
        if (provider == "github")
        {
            var github = HttpContext.RequestServices.GetRequiredService<IOptions<ExternalAuthSettings>>().Value.GitHub;
            var state = Guid.NewGuid().ToString("N");
            var clientIdEsc = Uri.EscapeDataString(github.ClientId);
            var scopeEsc = Uri.EscapeDataString("repo read:user user:email");

            string redirectUriToUse;
            if (!string.IsNullOrEmpty(github.FrontendRedirectUri))
            {
                redirectUriToUse = github.FrontendRedirectUri!;
            }
            else
            {
                redirectUriToUse = Url.ActionLink(action: "ExternalCallback", controller: "Accounts", values: new { provider = "github" }) ?? string.Empty;
            }

            var redirectUriEsc = Uri.EscapeDataString(redirectUriToUse);
            var authUrl = $"https://github.com/login/oauth/authorize?client_id={clientIdEsc}&redirect_uri={redirectUriEsc}&scope={scopeEsc}&state={state}";
            return Redirect(authUrl);
        }

        if (provider == "google")
        {
            var google = HttpContext.RequestServices.GetRequiredService<IOptions<ExternalAuthSettings>>().Value.Google;
            var state = Guid.NewGuid().ToString("N");
            var clientIdEsc = Uri.EscapeDataString(google.ClientId);
            
            string redirectUriToUse;
            if (!string.IsNullOrEmpty(google.FrontendRedirectUri))
            {
                redirectUriToUse = google.FrontendRedirectUri!;
            }
            else
            {
                redirectUriToUse = Url.ActionLink(action: "ExternalCallback", controller: "Accounts", values: new { provider = "google" }) ?? string.Empty;
            }
            
            var redirectUriEsc = Uri.EscapeDataString(redirectUriToUse);
            var scopeEsc = Uri.EscapeDataString("openid email profile");
            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientIdEsc}&redirect_uri={redirectUriEsc}&response_type=code&scope={scopeEsc}&state={state}&access_type=offline&prompt=consent";
            return Redirect(authUrl);
        }

        return BadRequestResponse("Unsupported provider");
    }

    [HttpGet("external/callback/{provider}")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalCallback(string provider, [FromQuery] string code, [FromQuery] string? state = null)
    {
        var frontendOrigin = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
            .GetValue<string>("FrontendOrigin") ?? "http://localhost:8080";

        if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(code))
            return Redirect($"{frontendOrigin}/login?error=missing_code");
        
        provider = provider.ToLower();
        
        try
        {
            if (provider == "github")
            {
                var command = new ExternalLoginCommand(Provider: "github", IdToken: string.Empty, AccessToken: null, AuthorizationCode: code);
                var result = await Mediator.Send(command);
                var accessTokenEnc = Uri.EscapeDataString(result.AccessToken);
                var refreshTokenEnc = Uri.EscapeDataString(result.RefreshToken);
                return Redirect($"{frontendOrigin}/auth/callback?accessToken={accessTokenEnc}&refreshToken={refreshTokenEnc}&provider=github&isNewUser={result.IsNewUser.ToString().ToLower()}");
            }

            if (provider == "google")
            {
                var googleSettings = HttpContext.RequestServices.GetRequiredService<IOptions<ExternalAuthSettings>>().Value.Google;
                var httpFactory = HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
                var client = httpFactory.CreateClient();

                string redirectUriToUse;
                if (!string.IsNullOrEmpty(googleSettings.FrontendRedirectUri))
                {
                    redirectUriToUse = googleSettings.FrontendRedirectUri!;
                }
                else
                {
                    redirectUriToUse = Url.ActionLink(action: "ExternalCallback", controller: "Accounts", values: new { provider = "google" }) ?? string.Empty;
                }

                var values = new Dictionary<string, string?>
                {
                    { "code", code },
                    { "client_id", googleSettings.ClientId },
                    { "client_secret", googleSettings.ClientSecret },
                    { "redirect_uri", redirectUriToUse },
                    { "grant_type", "authorization_code" }
                };

                var req = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token");
                req.Content = new FormUrlEncodedContent(values.Where(kv => kv.Value != null).ToDictionary(kv => kv.Key, kv => kv.Value!));

                var resp = await client.SendAsync(req);
                if (!resp.IsSuccessStatusCode)
                    return Redirect($"{frontendOrigin}/login?error=google_token_exchange_failed");

                var body = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                var idToken = root.TryGetProperty("id_token", out var idt) ? idt.GetString() : null;
                var accessToken = root.TryGetProperty("access_token", out var at) ? at.GetString() : null;

                if (string.IsNullOrEmpty(idToken))
                    return Redirect($"{frontendOrigin}/login?error=google_no_id_token");

                var command = new ExternalLoginCommand(Provider: "google", IdToken: idToken, AccessToken: accessToken, AuthorizationCode: null);
                var result = await Mediator.Send(command);
                var accessTokenEnc = Uri.EscapeDataString(result.AccessToken);
                var refreshTokenEnc = Uri.EscapeDataString(result.RefreshToken);
                return Redirect($"{frontendOrigin}/auth/callback?accessToken={accessTokenEnc}&refreshToken={refreshTokenEnc}&provider=google&isNewUser={result.IsNewUser.ToString().ToLower()}");
            }
        }
        catch (Exception ex)
        {
            var errorMsg = Uri.EscapeDataString(ex.Message);
            return Redirect($"{frontendOrigin}/login?error={errorMsg}");
        }

        return Redirect($"{frontendOrigin}/login?error=unsupported_provider");
    }
}
