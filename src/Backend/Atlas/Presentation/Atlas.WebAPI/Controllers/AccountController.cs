using System.Security.Claims;
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
using Atlas.Application.Features.Accounts.Queries.GetProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("fixed")]
public class AccountController(IMediator mediator) : ControllerBase
{
    #region Auth Endpoints (Public)
    
    [HttpPost("register")]
    [EnableRateLimiting("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("external-login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("verify-email")]
    [EnableRateLimiting("verification")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("verify-phone")]
    [EnableRateLimiting("verification")]
    public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("resend-email-verification-code")]
    [EnableRateLimiting("resend")]
    public async Task<IActionResult> ResendEmailVerificationCode([FromBody] ResendEmailVerificationCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("resend-phone-verification-code")]
    [EnableRateLimiting("resend")]
    public async Task<IActionResult> ResendPhoneVerificationCode([FromBody] ResendPhoneVerificationCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeAllTokenCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
    
    #endregion
    
    #region Profile Endpoints (Authorized)
    
    [Authorize]
    [HttpGet("profile")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await mediator.Send(new GetProfileQuery());
        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("profile")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("change-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("add-phone-number")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> AddPhoneNumber([FromBody] AddPhoneNumberCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [Authorize]
    [HttpDelete("delete-account")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> DeleteAccount(DeleteAccountCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("set-telegram-chat-id")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> SetTelegramChatId([FromBody] SetTelegramChatIdCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("generate-telegram-link-code")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> GenerateTelegramLinkCode(GenerateTelegramLinkCodeCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    #endregion
    
}
