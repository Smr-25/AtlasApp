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