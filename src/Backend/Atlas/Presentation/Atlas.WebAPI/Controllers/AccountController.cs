using System.Security.Claims;
using Atlas.Application.Dtos.Users;
using Atlas.Application.Dtos.Users.Auth;
using Atlas.Application.Dtos.Users.Profile;
using Atlas.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("fixed")]
public class AccountController(IAccountService service) : ControllerBase
{
    #region Auth Endpoints (Public)
    
    [HttpPost("register")]
    [EnableRateLimiting("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var result = await service.RegisterAsync(dto);
        return Ok(result);
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var result = await service.LoginAsync(dto);
        return Ok(result);
    }

    [HttpPost("external-login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> ExternalLogin([FromBody] UserExternalLoginDto dto)
    {
        var result = await service.ExternalLoginAsync(dto);
        return Ok(result);
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] UserRefreshTokenRequestDto dto)
    {
        await service.LogoutAsync(dto.RefreshToken);
        return Ok();
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<IActionResult> ForgotPassword([FromBody] UserForgotPasswordDto dto)
    {
        var result = await service.ForgotPasswordAsync(dto);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordDto dto)
    {
        var result = await service.ResetPasswordAsync(dto);
        return Ok(result);
    }

    [HttpPost("verify-email")]
    [EnableRateLimiting("verification")]
    public async Task<IActionResult> VerifyEmail([FromBody] UserVerifyEmailDto dto)
    {
        var result = await service.VerifyEmailAsync(dto);
        return Ok(result);
    }

    [HttpPost("verify-phone")]
    [EnableRateLimiting("verification")]
    public async Task<IActionResult> VerifyPhone([FromBody] UserVerifyPhoneDto dto)
    {
        var result = await service.VerifyPhoneAsync(dto);
        return Ok(result);
    }

    [HttpPost("resend-email-verification-code")]
    [EnableRateLimiting("resend")]
    public async Task<IActionResult> ResendEmailVerificationCode([FromBody] UserReverifyEmailDto dto)
    {
        var result = await service.ResendEmailVerificationCodeAsync(dto);
        return Ok(result);
    }

    [HttpPost("resend-phone-verification-code")]
    [EnableRateLimiting("resend")]
    public async Task<IActionResult> ResendPhoneVerificationCode([FromBody] UserReverifyPhoneDto dto)
    {
        var result = await service.ResendPhoneVerificationCodeAsync(dto);
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
    
    #endregion
    
    #region Profile Endpoints (Authorized)
    
    [Authorize]
    [HttpGet("profile")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var result = await service.GetProfileAsync(userId);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("profile")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await service.UpdateProfileAsync(userId, dto);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("change-password")]
    [EnableRateLimiting("password-reset")]
    public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await service.ChangePasswordAsync(userId, dto);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("add-phone-number")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> AddPhoneNumber([FromBody] UserAddPhoneNumberDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await service.AddPhoneNumberAsync(userId, dto);
        return Ok(result);
    }
    
    [Authorize]
    [HttpDelete("delete-account")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> DeleteAccount()
    {
        var userId = GetCurrentUserId();
        var result = await service.DeleteAccountAsync(userId);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("set-telegram-chat-id")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> SetTelegramChatId([FromBody] UserSetTelegramChatIdDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await service.SetTelegramChatIdAsync(userId, dto);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("generate-telegram-link-code")]
    [EnableRateLimiting("api")]
    public async Task<IActionResult> GenerateTelegramLinkCode()
    {
        var userId = GetCurrentUserId();
        var result = await service.GenerateTelegramLinkCodeAsync(userId);
        return Ok(result);
    }
    
    #endregion
    
    #region Private Methods
    
    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) 
               ?? throw new UnauthorizedAccessException("User not authenticated");
    }
    
    #endregion
}
