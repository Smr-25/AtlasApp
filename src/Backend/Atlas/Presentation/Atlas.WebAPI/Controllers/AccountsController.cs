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
            var redirectUri = Url.ActionLink(action: "ExternalCallback", controller: "Accounts", values: new { provider = "github" });
            var state = Guid.NewGuid().ToString("N");
            var authUrl = $"https://github.com/login/oauth/authorize?client_id={Uri.EscapeDataString(github.ClientId)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString("read:user user:email")}&state={state}";
            return Redirect(authUrl);
        }
        return BadRequestResponse("Unsupported provider");
    }

    [HttpGet("external/callback/{provider}")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalCallback(string provider, [FromQuery] string code, [FromQuery] string? state = null)
    {
        if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(code)) return BadRequestResponse("Missing provider or code");
        provider = provider.ToLower();
        if (provider != "github") return BadRequestResponse("Unsupported provider");

        var command = new ExternalLoginCommand(Provider: "github", IdToken: string.Empty, AccessToken: null, AuthorizationCode: code);
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
}
