using System.Security.Cryptography;
using Atlas.Application.Dtos.Users;
using Atlas.Application.Dtos.Users.Auth;
using Atlas.Application.Dtos.Users.Profile;
using Atlas.Application.Exceptions.Common;
using Atlas.Application.Exceptions.Users;
using Atlas.Application.Models;
using Atlas.Application.Services.Interfaces;
using Atlas.Application.Settings;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ValidationException = Atlas.Application.Exceptions.Common.ValidationException;

namespace Atlas.Application.Services.Concretes;

public class AccountService(
    UserManager<AppUser> userManager,
    IMapper mapper,
    IValidator<UserRegisterDto> registerValidator,
    IValidator<UserLoginDto> loginValidator,
    IValidator<UserForgotPasswordDto> forgotPasswordValidator,
    IValidator<UserResetPasswordDto> resetPasswordValidator,
    IValidator<UserVerifyEmailDto> verifyEmailValidator,
    IValidator<UserVerifyPhoneDto> verifyPhoneValidator,
    IValidator<UserAddPhoneNumberDto> addPhoneNumberValidator,
    IValidator<UserReverifyEmailDto> reverifyEmailValidator,
    IValidator<UserReverifyPhoneDto> reverifyPhoneValidator,
    IValidator<UserChangePasswordDto> changePasswordValidator,
    IValidator<UserProfileUpdateDto> profileUpdateValidator,
    IJwtService jwtService,
    IEmailService emailService,
    ISmsService smsService,
    ITelegramService telegramService,
    IExternalAuthService externalAuthService,
    IOptions<LockoutSettings> lockoutOptions
) : IAccountService
{
    private readonly LockoutSettings _lockoutSettings = lockoutOptions.Value;

    #region Authentication Methods

    public async Task<ResponseModel<bool>> RegisterAsync(UserRegisterDto userRegisterDto)
    {
        var validationResult = await registerValidator.ValidateAsync(userRegisterDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingUser = await userManager.FindByNameAsync(userRegisterDto.UserName);
        if (existingUser != null)
            throw new AlreadyExistException("User", userRegisterDto.UserName);

        var existingEmail = await userManager.FindByEmailAsync(userRegisterDto.Email);
        if (existingEmail != null)
            throw new AlreadyExistException("Email", userRegisterDto.Email);

        if (!string.IsNullOrEmpty(userRegisterDto.PhoneNumber))
        {
            var existingPhone = await userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == userRegisterDto.PhoneNumber);
            if (existingPhone != null)
                throw new AlreadyExistException("PhoneNumber", userRegisterDto.PhoneNumber);
        }

        var user = AppUser.Create(
            userRegisterDto.UserName,
            userRegisterDto.Email,
            userRegisterDto.FullName,
            userRegisterDto.PhoneNumber,
            userRegisterDto.PhoneVerificationChannel
        );

        var result = await userManager.CreateAsync(user, userRegisterDto.Password);

        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description));

        await SendEmailVerificationCodeAsync(userRegisterDto.Email);

        if (!string.IsNullOrEmpty(userRegisterDto.PhoneNumber) && userRegisterDto.PhoneVerificationChannel.HasValue)
            await SendPhoneVerificationCodeAsync(userRegisterDto.PhoneNumber!,
                userRegisterDto.PhoneVerificationChannel.Value);

        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<UserLoginResponseDto>> LoginAsync(UserLoginDto userLoginDto)
    {
        var validationResult = await loginValidator.ValidateAsync(userLoginDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await FindUserByEmailOrUserNameAsync(userLoginDto.Email, userLoginDto.UserName);
        if (user == null)
            throw new InvalidCredentialsException("Invalid UserName/Email or Password.");

        if (user.IsDeleted)
            throw new UnauthorizedException("This account has been deleted.");

        if (user.IsLockedOut)
        {
            var remainingTime = user.LockoutEndTime!.Value - DateTime.UtcNow;
            throw new AccountLockedException(
                $"Account is locked. Try again in {(int)remainingTime.TotalMinutes} minutes.");
        }

        if (!user.EmailConfirmed)
            throw new EmailNotVerifiedException("Email is not verified. Please verify your email before logging in.");

        var passwordValid = await userManager.CheckPasswordAsync(user, userLoginDto.Password);
        if (!passwordValid)
        {
            user.IncrementFailedLoginAttempts(
                _lockoutSettings.MaxFailedAccessAttempts,
                TimeSpan.FromMinutes(_lockoutSettings.LockoutDurationInMinutes));
            await userManager.UpdateAsync(user);

            if (user.IsLockedOut)
                throw new AccountLockedException(
                    $"Too many failed attempts. Account locked for {_lockoutSettings.LockoutDurationInMinutes} minutes.");

            var remainingAttempts = _lockoutSettings.MaxFailedAccessAttempts - user.FailedLoginAttempts;
            throw new InvalidCredentialsException(
                $"Invalid UserName/Email or Password. {remainingAttempts} attempts remaining before account lockout.");
        }

        user.ResetFailedLoginAttempts();
        await userManager.UpdateAsync(user);


        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshTokenResponse(user);
        user.SetRefreshToken(refreshToken.RefreshToken, refreshToken.RefreshTokenExpiresAt);
        user.UpdateLastLogin();
        await userManager.UpdateAsync(user);

        var loginResponse = new UserLoginResponseDto
        (
            accessToken,
            refreshToken.RefreshToken,
            user.UserName!,
            DateTime.UtcNow,
            refreshToken.RefreshTokenExpiresAt
        );
        return ResponseModel<UserLoginResponseDto>.Success(loginResponse);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            throw new BadRequestException("Refresh token is required.");

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user == null)
            throw new InvalidCredentialsException("Invalid refresh token.");

        user.RevokeRefreshToken();
        await userManager.UpdateAsync(user);
    }

    public async Task<ResponseModel<UserExternalLoginReturnDto>> ExternalLoginAsync(
        UserExternalLoginDto userExternalLoginDto)
    {
        var externalUser = userExternalLoginDto.Provider.ToLower() switch
        {
            "apple" => await externalAuthService.ValidateAppleTokenAsync(userExternalLoginDto.IdToken),
            "google" => await externalAuthService.ValidateGoogleTokenAsync(userExternalLoginDto.IdToken),
            _ => throw new BadRequestException("Unsupported external authentication provider. Supported: Google, Apple")
        };

        if (externalUser == null)
            throw new InvalidCredentialsException("Invalid external authentication token.");

        var (user, isNewUser) = await FindOrCreateExternalUserAsync(externalUser);

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshTokenResponse = jwtService.GenerateRefreshTokenResponse(user);

        user.SetRefreshToken(refreshTokenResponse.RefreshToken, refreshTokenResponse.RefreshTokenExpiresAt);
        user.UpdateLastLogin();
        await userManager.UpdateAsync(user);

        var externalReturnDto = new UserExternalLoginReturnDto
        (
            AccessToken: accessToken,
            RefreshToken: refreshTokenResponse.RefreshToken,
            IsNewUser: isNewUser,
            UserId: user.Id,
            Email: user.Email!,
            FullName: user.FullName,
            RefreshTokenExpiryTime: refreshTokenResponse.RefreshTokenExpiresAt
        );
        return ResponseModel<UserExternalLoginReturnDto>.Success(externalReturnDto);
    }

    #endregion

    #region Token Management Methods

    public async Task<ResponseModel<UserRefreshTokenResponseDto>> RefreshTokenAsync(
        UserRefreshTokenRequestDto userRefreshTokenRequestDto)
    {
        if (string.IsNullOrEmpty(userRefreshTokenRequestDto.RefreshToken))
            throw new InvalidCredentialsException("Refresh token is required.");

        var user = await userManager.Users.FirstOrDefaultAsync(u =>
            u.RefreshToken == userRefreshTokenRequestDto.RefreshToken);

        if (user == null || user.RefreshToken != userRefreshTokenRequestDto.RefreshToken ||
            user.RefreshTokenExpiresAt < DateTime.UtcNow)
            throw new InvalidCredentialsException("Invalid refresh token.");

        var newAccessToken = jwtService.GenerateAccessToken(user);
        var newRefreshToken = jwtService.GenerateRefreshTokenResponse(user);

        user.SetRefreshToken(newRefreshToken.RefreshToken, newRefreshToken.RefreshTokenExpiresAt);
        await userManager.UpdateAsync(user);

        var refreshTokenResponse = new UserRefreshTokenResponseDto
        (
            newAccessToken,
            newRefreshToken.RefreshToken,
            newRefreshToken.RefreshTokenExpiresAt
        );
        return ResponseModel<UserRefreshTokenResponseDto>.Success(refreshTokenResponse);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            throw new BadRequestException("Refresh token is required.");

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user == null)
            throw new InvalidCredentialsException("Invalid refresh token.");

        user.RevokeRefreshToken();
        await userManager.UpdateAsync(user);
    }

    #endregion

    #region Password Management Methods

    public async Task<ResponseModel<bool>> ForgotPasswordAsync(UserForgotPasswordDto userForgotPasswordDto)
    {
        var validationResult = await forgotPasswordValidator.ValidateAsync(userForgotPasswordDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await FindUserByEmailOrUserNameAsync(userForgotPasswordDto.Email,
            userForgotPasswordDto.UserName);

        if (user == null)
            return ResponseModel<bool>.Success(true);

        await SendPasswordResetEmailAsync(user.Email!);
        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<bool>> ResetPasswordAsync(UserResetPasswordDto userResetPasswordDto)
    {
        var validationResult = await resetPasswordValidator.ValidateAsync(userResetPasswordDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await FindUserByEmailOrUserNameAsync(userResetPasswordDto.Email, userResetPasswordDto.UserName);

        if (user == null)
        {
            var identifier = !string.IsNullOrEmpty(userResetPasswordDto.Email)
                ? userResetPasswordDto.Email
                : userResetPasswordDto.UserName ?? "unknown";
            throw new NotFoundException("User", identifier);
        }

        if (user.ResetPasswordCode != userResetPasswordDto.Code ||
            user.ResetPasswordExpiresAt < DateTime.UtcNow)
            throw new InvalidOrExpiredCodeException("Password Reset");

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, resetToken, userResetPasswordDto.NewPassword);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description));

        user.ResetPasswordCode = null;
        user.ResetPasswordExpiresAt = null;
        await userManager.UpdateAsync(user);

        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<bool>> ChangePasswordAsync(string userId,
        UserChangePasswordDto userChangePasswordDto)
    {
        var validationResult = await changePasswordValidator.ValidateAsync(userChangePasswordDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        var passwordValid = await userManager.CheckPasswordAsync(user, userChangePasswordDto.CurrentPassword);
        if (!passwordValid)
            throw new InvalidCredentialsException("Current password is incorrect.");

        var result = await userManager.ChangePasswordAsync(user, userChangePasswordDto.CurrentPassword,
            userChangePasswordDto.NewPassword);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description));

        return ResponseModel<bool>.Success(true);
    }

    #endregion

    #region Verification Methods

    public async Task<ResponseModel<bool>> VerifyEmailAsync(UserVerifyEmailDto userVerifyEmailDto)
    {
        var validationResult = await verifyEmailValidator.ValidateAsync(userVerifyEmailDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await userManager.FindByEmailAsync(userVerifyEmailDto.Email);

        if (user == null)
            throw new NotFoundException("User", userVerifyEmailDto.Email);

        if (user.EmailVerificationCode != userVerifyEmailDto.Code ||
            user.EmailVerificationExpiresAt < DateTime.UtcNow)
            throw new InvalidOrExpiredCodeException("Email Verification");

        user.EmailConfirmed = true;
        user.EmailVerificationCode = null;
        user.EmailVerificationExpiresAt = null;

        if (string.IsNullOrEmpty(user.PhoneNumber) || user.PhoneNumberConfirmed)
            user.Activate();

        await userManager.UpdateAsync(user);
        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<bool>> VerifyPhoneAsync(UserVerifyPhoneDto userVerifyPhoneDto)
    {
        var validationResult = await verifyPhoneValidator.ValidateAsync(userVerifyPhoneDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await FindUserByPhoneNumberAsync(userVerifyPhoneDto.PhoneNumber);

        if (user == null)
            throw new NotFoundException("User", userVerifyPhoneDto.PhoneNumber);

        if (user.PhoneVerificationCode != userVerifyPhoneDto.Code ||
            user.PhoneVerificationExpiresAt < DateTime.UtcNow)
            throw new InvalidOrExpiredCodeException("Phone Verification");

        user.PhoneNumberConfirmed = true;
        user.PhoneVerificationCode = null;
        user.PhoneVerificationExpiresAt = null;

        if (user.EmailConfirmed)
            user.Activate();

        await userManager.UpdateAsync(user);
        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<bool>> ResendEmailVerificationCodeAsync(UserReverifyEmailDto userReverifyEmailDto)
    {
        var validationResult = await reverifyEmailValidator.ValidateAsync(userReverifyEmailDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await userManager.FindByEmailAsync(userReverifyEmailDto.Email);
        if (user == null)
            throw new NotFoundException("User", userReverifyEmailDto.Email);

        if (user.EmailConfirmed)
            throw new AlreadyVerifiedException("Email");

        var code = GenerateVerificationCode();
        user.EmailVerificationCode = code;
        user.EmailVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendVerificationEmailAsync(user.Email!, code);

        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<bool>> ResendPhoneVerificationCodeAsync(UserReverifyPhoneDto userReverifyPhoneDto)
    {
        var validationResult = await reverifyPhoneValidator.ValidateAsync(userReverifyPhoneDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await FindUserByPhoneNumberAsync(userReverifyPhoneDto.PhoneNumber);
        if (user == null)
            throw new NotFoundException("User", userReverifyPhoneDto.PhoneNumber);

        if (user.PhoneNumberConfirmed)
            throw new AlreadyVerifiedException("Phone Number");

        var code = GenerateVerificationCode();
        user.PhoneVerificationCode = code;
        user.PhoneVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);

        switch (userReverifyPhoneDto.UserVerificationChannel)
        {
            case UserVerificationChannel.Sms:
                await smsService.SendVerificationSmsAsync(user.PhoneNumber!, code);
                break;
            case UserVerificationChannel.Telegram:
                if (string.IsNullOrEmpty(user.TelegramChatId))
                    throw new BadRequestException("Telegram account is not linked. Please link your Telegram first.");
                await telegramService.SendVerificationCodeAsync(user.TelegramChatId, code);
                break;
            default:
                throw new InvalidVerificationChannelException();
        }

        return ResponseModel<bool>.Success(true);
    }


    #endregion

    #region Profile Management Methods

    public async Task<ResponseModel<UserProfileReturnDto>> GetProfileAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        var userProfile = mapper.Map<UserProfileReturnDto>(user);
        return ResponseModel<UserProfileReturnDto>.Success(userProfile);
    }

    public async Task<ResponseModel<UserProfileUpdateDto>> UpdateProfileAsync(string userId,
        UserProfileUpdateDto userProfileUpdateDto)
    {
        var validationResult = await profileUpdateValidator.ValidateAsync(userProfileUpdateDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        if (!string.IsNullOrEmpty(userProfileUpdateDto.UserName) && user.UserName != userProfileUpdateDto.UserName)
        {
            var existingUserName = await userManager.FindByNameAsync(userProfileUpdateDto.UserName);
            if (existingUserName != null)
                throw new AlreadyExistException("UserName", userProfileUpdateDto.UserName);
        }

        user.UpdateProfile(userProfileUpdateDto.FullName, userProfileUpdateDto.UserName);

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description));

        return ResponseModel<UserProfileUpdateDto>.Success(userProfileUpdateDto);
    }

    public async Task<ResponseModel<bool>> AddPhoneNumberAsync(string userId,
        UserAddPhoneNumberDto userAddPhoneNumberDto)
    {
        var validationResult = await addPhoneNumberValidator.ValidateAsync(userAddPhoneNumberDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        if (!string.IsNullOrEmpty(user.PhoneNumber))
            throw new BadRequestException("Phone number already exists. Use update phone number instead.");

        var existingPhone = await FindUserByPhoneNumberAsync(userAddPhoneNumberDto.PhoneNumber);
        if (existingPhone != null)
            throw new AlreadyExistException("PhoneNumber", userAddPhoneNumberDto.PhoneNumber);

        user.PhoneNumber = userAddPhoneNumberDto.PhoneNumber;
        user.PreferredVerificationChannel = userAddPhoneNumberDto.UserVerificationChannel;
        await userManager.UpdateAsync(user);

        await SendPhoneVerificationCodeAsync(userAddPhoneNumberDto.PhoneNumber,
            userAddPhoneNumberDto.UserVerificationChannel);

        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<bool>> DeleteAccountAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        user.MarkAsDeleted();
        await userManager.UpdateAsync(user);
        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<bool>> SetTelegramChatIdAsync(string userId, UserSetTelegramChatIdDto dto)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        user.TelegramChatId = dto.TelegramChatId;
        await userManager.UpdateAsync(user);

        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<string>> GenerateTelegramLinkCodeAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        var linkCode = Guid.NewGuid().ToString("N")[..8].ToUpper();
        user.TelegramLinkCode = linkCode;
        user.TelegramLinkCodeExpiry = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);

        return ResponseModel<string>.Success(linkCode);
    }
    
    public async Task LinkTelegramByChatIdAsync(string linkCode, string chatId)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => 
            u.TelegramLinkCode == linkCode && 
            u.TelegramLinkCodeExpiry > DateTime.UtcNow);

        if (user == null)
            return; 

        user.TelegramChatId = chatId;
        user.TelegramLinkCode = null;
        user.TelegramLinkCodeExpiry = null;
        await userManager.UpdateAsync(user);
    }

    #endregion

    #region Private Helper Methods

    private async Task SendEmailVerificationCodeAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            throw new NotFoundException("User", email);

        var code = GenerateVerificationCode();
        user.EmailVerificationCode = code;
        user.EmailVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendVerificationEmailAsync(user.Email!, code);
    }

    private async Task SendPhoneVerificationCodeAsync(string phoneNumber, UserVerificationChannel channel)
    {
        var user = await FindUserByPhoneNumberAsync(phoneNumber);
        if (user == null)
            throw new NotFoundException("User", phoneNumber);

        var code = GenerateVerificationCode();
        user.PhoneVerificationCode = code;
        user.PhoneVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        user.PreferredVerificationChannel = channel;
        await userManager.UpdateAsync(user);

        switch (channel)
        {
            case UserVerificationChannel.Sms:
                await smsService.SendVerificationSmsAsync(user.PhoneNumber!, code);
                break;
            case UserVerificationChannel.Telegram:
                if (string.IsNullOrEmpty(user.TelegramChatId))
                    throw new BadRequestException("Telegram account is not linked. Please link your Telegram first.");
                await telegramService.SendVerificationCodeAsync(user.TelegramChatId, code);
                break;
            default:
                throw new InvalidVerificationChannelException();
        }
    }

    private async Task SendPasswordResetEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            throw new NotFoundException("User", email);

        var code = GenerateVerificationCode();
        user.ResetPasswordCode = code;
        user.ResetPasswordExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendPasswordResetEmailAsync(user.Email!, code);
    }

    private async Task<AppUser?> FindUserByEmailOrUserNameAsync(string? email, string? userName)
    {
        if (!string.IsNullOrEmpty(email))
            return await userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (!string.IsNullOrEmpty(userName))
            return await userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);

        return null;
    }

    private async Task<AppUser?> FindUserByPhoneNumberAsync(string phoneNumber)
    {
        return await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    private static string GenerateVerificationCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var code = (BitConverter.ToUInt32(bytes, 0) % 900000 + 100000).ToString();
        return code;
    }

    private async Task<(AppUser User, bool IsNewUser)> FindOrCreateExternalUserAsync(ExternalUserInfo externalUser)
    {
        var user = await userManager.FindByLoginAsync(externalUser.Provider, externalUser.ProviderId);

        if (user is not null)
            return (user, false);

        user = await userManager.FindByEmailAsync(externalUser.Email);

        if (user is not null)
        {
            await userManager.AddLoginAsync(user, new UserLoginInfo(
                externalUser.Provider,
                externalUser.ProviderId,
                externalUser.Provider
            ));
            return (user, false);
        }

        var userName = GenerateUserNameFromEmail(externalUser.Email);
        var fullName = externalUser.FullName ?? externalUser.Email.Split('@')[0];
        
        user = AppUser.Create(
            userName: userName,
            email: externalUser.Email,
            fullName: fullName
        );
        
        if (externalUser.EmailVerified)
        {
            user.EmailConfirmed = true;
            user.Activate();
        }
        
        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description));
        
        await userManager.AddLoginAsync(user, new UserLoginInfo(
            externalUser.Provider,
            externalUser.ProviderId,
            externalUser.Provider
        ));

        return (user, true);
    }

    private static string GenerateUserNameFromEmail(string email)
    {
        var baseUserName = email.Split('@')[0];
        var random = Random.Shared;
        return $"{baseUserName}{random.Next(1000, 9999)}";
    }
    
    #endregion
}