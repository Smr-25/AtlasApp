using Atlas.Application.Dtos.Users;
using Atlas.Application.Models;

namespace Atlas.Application.Services.Interfaces;

public interface IAccountService
{
    Task<ResponseModel<bool>> RegisterAsync(UserRegisterDto userRegisterDto);
    Task<ResponseModel<UserLoginResponseDto>> LoginAsync(UserLoginDto userLoginDto);
    Task<ResponseModel<bool>> ForgotPasswordAsync(UserForgotPasswordDto userForgotPasswordDto);
    Task<ResponseModel<bool>> ResetPasswordAsync(UserResetPasswordDto userResetPasswordDto);
    Task<ResponseModel<bool>> VerifyAccountAsync(UserVerifyEmailDto userVerifyEmailDto);
    Task<ResponseModel<bool>> ResendVerificationCodeAsync(UserVerifyEmailDto userVerifyEmailDto);
}