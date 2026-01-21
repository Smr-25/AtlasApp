using Atlas.Application.Dtos.Users;
using Atlas.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IAccountService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var result = await service.RegisterAsync(dto);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var result = await service.LoginAsync(dto);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] UserForgotPasswordDto dto)
    {
        var result = await service.ForgotPasswordAsync(dto);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordDto dto)
    {
        var result = await service.ResetPasswordAsync(dto);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] UserVerifyEmailDto dto)
    {
        var result = await service.VerifyEmailAsync(dto);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }

    [HttpPost("verify-phone")]
    public async Task<IActionResult> VerifyPhone([FromBody] UserVerifyPhoneDto dto)
    {
        var result = await service.VerifyPhoneAsync(dto);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }

    [HttpPost("add-phone-number")]
    public async Task<IActionResult> AddPhoneNumber([FromBody] UserAddPhoneNumberDto dto)
    {
        var result = await service.AddPhoneNumberAsync(dto);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }

    [HttpPost("resend-email-verification-code")]
    public async Task<IActionResult> ResendEmailVerificationCode([FromBody] string email)
    {
        var result = await service.ResendEmailVerificationCodeAsync(email);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }

    [HttpPost("resend-phone-verification-code")]
    public async Task<IActionResult> ResendPhoneVerificationCode([FromBody] string phoneNumber)
    {
        var result = await service.ResendPhoneVerificationCodeAsync(phoneNumber);
        return !result.IsSuccess ? BadRequest(result) : Ok(result);
    }
    
}