using Atlas.Application.Dtos.Users;
using Atlas.Application.Dtos.Users.Auth;
using Atlas.Application.Dtos.Users.Profile;
using Atlas.Application.Models;

namespace Atlas.Application.Services.Interfaces;

public interface IAccountService
{
    Task<ResponseModel<bool>> RegisterAsync(UserRegisterDto userRegisterDto);
    Task<ResponseModel<UserLoginResponseDto>> LoginAsync(UserLoginDto userLoginDto);
    Task<ResponseModel<UserRefreshTokenResponseDto>> RefreshTokenAsync(UserRefreshTokenRequestDto userRefreshTokenRequestDto);
    Task RevokeRefreshTokenAsync(string refreshToken);
    Task<ResponseModel<UserExternalLoginReturnDto>> ExternalLoginAsync(UserExternalLoginDto userExternalLoginDto);
    Task<ResponseModel<bool>> ForgotPasswordAsync(UserForgotPasswordDto userForgotPasswordDto);
    Task<ResponseModel<bool>> ResetPasswordAsync(UserResetPasswordDto userResetPasswordDto);
    Task<ResponseModel<bool>> VerifyEmailAsync(UserVerifyEmailDto userVerifyEmailDto);
    Task<ResponseModel<bool>> VerifyPhoneAsync(UserVerifyPhoneDto userVerifyPhoneDto);
    Task<ResponseModel<bool>> ResendEmailVerificationCodeAsync(UserReverifyEmailDto userReverifyEmailDto);
    Task<ResponseModel<bool>> ResendPhoneVerificationCodeAsync(UserReverifyPhoneDto userReverifyPhoneDto);
    Task<ResponseModel<UserTelegramResponseDto>> GenerateTelegramLinkAsync(string email);
    Task LogoutAsync(string refreshToken);
    Task<ResponseModel<UserProfileReturnDto>> GetProfileAsync(string userId);
    Task<ResponseModel<UserProfileUpdateDto>> UpdateProfileAsync(string userId, UserProfileUpdateDto userProfileUpdateDto);
    Task<ResponseModel<bool>> ChangePasswordAsync(string userId,UserChangePasswordDto userChangePasswordDto);
    Task<ResponseModel<bool>> AddPhoneNumberAsync(string userId, UserAddPhoneNumberDto userAddPhoneNumberDto);
    Task<ResponseModel<bool>> DeleteAccountAsync(string userId);
}