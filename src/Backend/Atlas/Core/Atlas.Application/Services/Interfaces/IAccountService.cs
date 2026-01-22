using Atlas.Application.Dtos.Users;
using Atlas.Application.Models;

namespace Atlas.Application.Services.Interfaces;

public interface IAccountService
{
    Task<ResponseModel<bool>> RegisterAsync(UserRegisterDto userRegisterDto);
    Task<ResponseModel<UserLoginResponseDto>> LoginAsync(UserLoginDto userLoginDto);
    Task<ResponseModel<UserExternalLoginResultDto>> ExternalLoginAsync(UserExternalLoginDto userExternalLoginDto);
    Task<ResponseModel<bool>> ForgotPasswordAsync(UserForgotPasswordDto userForgotPasswordDto);
    Task<ResponseModel<bool>> ResetPasswordAsync(UserResetPasswordDto userResetPasswordDto);
    Task<ResponseModel<bool>> VerifyEmailAsync(UserVerifyEmailDto userVerifyEmailDto);
    Task<ResponseModel<bool>> VerifyPhoneAsync(UserVerifyPhoneDto userVerifyPhoneDto);
    Task<ResponseModel<bool>> AddPhoneNumberAsync(UserAddPhoneNumberDto userAddPhoneNumberDto);
    Task<ResponseModel<bool>> ResendEmailVerificationCodeAsync(UserReverifyEmailDto userReverifyEmailDto);
    Task<ResponseModel<bool>> ResendPhoneVerificationCodeAsync(UserReverifyPhoneDto userReverifyPhoneDto);
    
}