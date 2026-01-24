using Atlas.Application.Dtos.Users;
using Atlas.Application.Dtos.Users.Auth;
using Atlas.Application.Dtos.Users.Profile;
using Atlas.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/Auth")]
public class AccountController(IAccountService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var result = await service.RegisterAsync(dto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var result = await service.LoginAsync(dto);
        return Ok(result);
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        await service.LogoutAsync(refreshToken);
        return Ok();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] UserForgotPasswordDto dto)
    {
        var result = await service.ForgotPasswordAsync(dto);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordDto dto)
    {
        var result = await service.ResetPasswordAsync(dto);
        return Ok(result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] UserVerifyEmailDto dto)
    {
        var result = await service.VerifyEmailAsync(dto);
        return Ok(result);
    }

    [HttpPost("verify-phone")]
    public async Task<IActionResult> VerifyPhone([FromBody] UserVerifyPhoneDto dto)
    {
        var result = await service.VerifyPhoneAsync(dto);
        return Ok(result);
    }

    [HttpPost("add-phone-number")]
    public async Task<IActionResult> AddPhoneNumber([FromBody] UserAddPhoneNumberDto dto)
    {
        var result = await service.AddPhoneNumberAsync(dto);
        return Ok(result);
    }

    [HttpPost("resend-email-verification-code")]
    public async Task<IActionResult> ResendEmailVerificationCode([FromBody] UserReverifyEmailDto dto)
    {
        var result = await service.ResendEmailVerificationCodeAsync(dto);
        return Ok(result);
    }

    [HttpPost("resend-phone-verification-code")]
    public async Task<IActionResult> ResendPhoneVerificationCode([FromBody] UserReverifyPhoneDto dto)
    {
        var result = await service.ResendPhoneVerificationCodeAsync(dto);
        return Ok(result);
    }

    [HttpPost("generate-telegram-link")]
    public async Task<IActionResult> GenerateTelegramLink([FromBody] UserLinkTelegramDto dto)
    {
        var result = await service.GenerateTelegramLinkAsync(dto.Email);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] UserRefreshTokenRequestDto dto)
    {
        var result = await service.RefreshTokenAsync(dto);
        return Ok(result);
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] UserRefreshTokenRequestDto dto)
    {
        await service.RevokeRefreshTokenAsync(dto.RefreshToken);
        return Ok();
    }
}