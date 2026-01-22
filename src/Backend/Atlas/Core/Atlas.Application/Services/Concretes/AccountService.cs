using Atlas.Application.Dtos.Users;
using Atlas.Application.Interfaces;
using Atlas.Application.Models;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using AutoMapper;
using FluentValidation;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Services.Concretes;

public class AccountService(
    IApplicationDbContext applicationDbContext,
    IMapper mapper,
    UserManager<AppUser> userManager,
    IValidator<UserRegisterDto> registerValidator,
    IValidator<UserLoginDto> loginValidator,
    IValidator<UserForgotPasswordDto> forgotPasswordValidator,
    IValidator<UserResetPasswordDto> resetPasswordValidator,
    IValidator<UserVerifyEmailDto> verifyEmailValidator,
    IValidator<UserVerifyPhoneDto> verifyPhoneValidator,
    IValidator<UserAddPhoneNumberDto> addPhoneNumberValidator,
    IValidator<UserReverifyEmailDto> reverifyEmailValidator,
    IValidator<UserReverifyPhoneDto> reverifyPhoneValidator,
    IJwtService jwtService,
    IEmailService emailService,
    ISmsService smsService,
    ITelegramService telegramService
) : IAccountService
{
    public async Task<ResponseModel<bool>> RegisterAsync(UserRegisterDto userRegisterDto)
    {
        var validationResult = await registerValidator.ValidateAsync(userRegisterDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingUser = await userManager.FindByNameAsync(userRegisterDto.UserName);
        if (existingUser != null)
            return ResponseModel<bool>.Failure("User already exists.");

        var existingEmail = await userManager.FindByEmailAsync(userRegisterDto.Email);
        if (existingEmail != null)
            return ResponseModel<bool>.Failure("Email already in use.");

        if (string.IsNullOrEmpty(userRegisterDto.PhoneNumber))
        {
            var existingPhone = applicationDbContext.Users
                .FirstOrDefault(u => u.PhoneNumber == userRegisterDto.PhoneNumber);
            if (existingPhone != null)
                return ResponseModel<bool>.Failure("Phone number already in use.");
        }

        var user = mapper.Map<AppUser>(userRegisterDto);
        var result = await userManager.CreateAsync(user, userRegisterDto.Password);

        if (!result.Succeeded)
            return ResponseModel<bool>.Failure(result.Errors.Select(e => e.Description));

        await SendEmailVerificationCodeAsync(userRegisterDto.Email);

        if(!string.IsNullOrEmpty(userRegisterDto.PhoneNumber) && userRegisterDto.PhoneVerificationChannel.HasValue)
            await SendPhoneVerificationCodeAsync(userRegisterDto.PhoneNumber!,userRegisterDto.PhoneVerificationChannel.Value);

        return !result.Succeeded
            ? ResponseModel<bool>.Failure(result.Errors.Select(e => e.Description))
            : ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<UserLoginResponseDto>> LoginAsync(UserLoginDto userLoginDto)
    {
        var validationResult = await loginValidator.ValidateAsync(userLoginDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await FindUserByEmailOrUserNameAsync(userLoginDto.Email,
            userLoginDto.UserName);
        if (user == null)
            return ResponseModel<UserLoginResponseDto>.Failure("Invalid username or password.");

        if(!user.EmailConfirmed)
            return ResponseModel<UserLoginResponseDto>.Failure("Email is not verified.");
            
        var passwordValid = await userManager.CheckPasswordAsync(user, userLoginDto.Password);
        if (!passwordValid)
            return ResponseModel<UserLoginResponseDto>.Failure("Invalid username or password.");

        var token = jwtService.GenerateToken(user);
        var loginResponse = new UserLoginResponseDto
        {
            Token = token,
            UserName = user.UserName!,
            ExpiresAt = DateTime.UtcNow.AddHours(3)
        };
        return ResponseModel<UserLoginResponseDto>.Success(loginResponse);
    }

    public Task<ResponseModel<UserExternalLoginResultDto>> ExternalLoginAsync(UserExternalLoginDto userExternalLoginDto)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseModel<bool>> ForgotPasswordAsync(UserForgotPasswordDto userForgotPasswordDto)
    {
        var validationResult = await forgotPasswordValidator.ValidateAsync(userForgotPasswordDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await FindUserByEmailOrUserNameAsync(userForgotPasswordDto.Email,
            userForgotPasswordDto.UserName);

        if (user == null)
            return ResponseModel<bool>.Success(true);

        return await SendPasswordResetEmailAsync(user.Email!);
    }

    public async Task<ResponseModel<bool>> ResetPasswordAsync(UserResetPasswordDto userResetPasswordDto)
    {
        var validationResult = await resetPasswordValidator.ValidateAsync(userResetPasswordDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await FindUserByEmailOrUserNameAsync(userResetPasswordDto.Email, userResetPasswordDto.UserName);

        if (user == null)
            return ResponseModel<bool>.Failure("User not found.");

        if (user.ResetPasswordCode != userResetPasswordDto.Code ||
            user.ResetPasswordExpiresAt < DateTime.UtcNow)

            return ResponseModel<bool>.Failure("Invalid or expired reset code.");


        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, resetToken, userResetPasswordDto.NewPassword);
        if (!result.Succeeded)
            return ResponseModel<bool>.Failure(result.Errors.Select(e => e.Description));

        user.ResetPasswordCode = null;
        user.ResetPasswordExpiresAt = null;
        await userManager.UpdateAsync(user);

        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<bool>> VerifyEmailAsync(UserVerifyEmailDto userVerifyEmailDto)
    {
        var validationResult = await verifyEmailValidator.ValidateAsync(userVerifyEmailDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await userManager.FindByEmailAsync(userVerifyEmailDto.Email);

        if (user == null)
            return ResponseModel<bool>.Failure("User not found.");

        if (user.EmailVerificationCode != userVerifyEmailDto.Code ||
            user.EmailVerificationExpiresAt < DateTime.UtcNow)

            return ResponseModel<bool>.Failure("Invalid or expired verification code.");

        user.EmailConfirmed = true;
        user.EmailVerificationCode = null;
        user.EmailVerificationExpiresAt = null;
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
            return ResponseModel<bool>.Failure("User not found.");

        if (user.PhoneVerificationCode != userVerifyPhoneDto.Code ||
            user.PhoneVerificationExpiresAt < DateTime.UtcNow)

            return ResponseModel<bool>.Failure("Invalid or expired verification code.");

        user.PhoneNumberConfirmed = true;
        user.PhoneVerificationCode = null;
        user.PhoneVerificationExpiresAt = null;
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
            return ResponseModel<bool>.Failure("User not found.");
        if (user.EmailConfirmed)
            return ResponseModel<bool>.Failure("Email is already verified.");
        var code = await GenerateVerificationCodeAsync();
        user.EmailVerificationCode = code;
        user.EmailVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendVerificationEmailAsync(user.Email!,
            code);
        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<bool>> ResendPhoneVerificationCodeAsync(UserReverifyPhoneDto userReverifyPhoneDto)
    {
        var validationResult = await reverifyPhoneValidator.ValidateAsync(userReverifyPhoneDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var user = await FindUserByPhoneNumberAsync(userReverifyPhoneDto.PhoneNumber);
        if (user == null)
            return ResponseModel<bool>.Failure("User not found.");
        if (user.PhoneNumberConfirmed)
            return ResponseModel<bool>.Failure("Phone number is already verified.");

        var code = await GenerateVerificationCodeAsync();
        user.PhoneVerificationCode = code;
        user.PhoneVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);

        await smsService.SendVerificationSmsAsync(user.PhoneNumber!,
            $"Your verification code is: {code}");
        return ResponseModel<bool>.Success(true);
    }
    
    public async Task<ResponseModel<bool>> AddPhoneNumberAsync(UserAddPhoneNumberDto userAddPhoneNumberDto)
    {
        var validationResult = await addPhoneNumberValidator.ValidateAsync(userAddPhoneNumberDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await userManager.FindByEmailAsync(userAddPhoneNumberDto.Email);
        if (user == null)
            return ResponseModel<bool>.Failure("User not found.");

        var existingPhone = await FindUserByPhoneNumberAsync(userAddPhoneNumberDto.PhoneNumber);
        if (existingPhone != null)
            return ResponseModel<bool>.Failure("Phone number already in use.");

        user.PhoneNumber = userAddPhoneNumberDto.PhoneNumber;
        user.PreferredVerificationChannel = userAddPhoneNumberDto.UserVerificationChannel;
        await userManager.UpdateAsync(user);
        
        await SendPhoneVerificationCodeAsync(userAddPhoneNumberDto.PhoneNumber,userAddPhoneNumberDto.UserVerificationChannel);
        
        return ResponseModel<bool>.Success(true);
    }
    
public async Task<ResponseModel<UserTelegramResponseDto>> GenerateTelegramLinkAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            return ResponseModel<UserTelegramResponseDto>.Failure("User not found.");
    
        var linkCode = Guid.NewGuid().ToString("N")[..8].ToUpper();
        user.TelegramLinkCode = linkCode;
        user.TelegramLinkCodeExpiry = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
    
        var botLink = await telegramService.GetBotLinkAsync(linkCode);
    
        return ResponseModel<UserTelegramResponseDto>.Success(
            new UserTelegramResponseDto(botLink, linkCode));
    }
    

    private async Task<ResponseModel<bool>> SendEmailVerificationCodeAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            return ResponseModel<bool>.Failure("User not found.");

        var code = await GenerateVerificationCodeAsync();
        user.EmailVerificationCode = code;
        user.EmailVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendVerificationEmailAsync(user.Email!,
            code);
        return ResponseModel<bool>.Success(true);
    }

    private async Task<ResponseModel<bool>> SendPhoneVerificationCodeAsync(string phoneNumber, UserVerificationChannel channel)
    {
        var user = await FindUserByPhoneNumberAsync(phoneNumber);
        if (user == null)
            return ResponseModel<bool>.Failure("User not found.");

        var code = await GenerateVerificationCodeAsync();
        user.PhoneVerificationCode = code;
        user.PhoneVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        user.PreferredVerificationChannel = channel;
        await userManager.UpdateAsync(user);

        switch (channel)
        {
            case UserVerificationChannel.Sms:
                await smsService.SendVerificationSmsAsync(user.PhoneNumber!,
                    $"Your verification code is: {code}");
                break;
            case UserVerificationChannel.Telegram:
                await telegramService.SendVerificationCodeAsync(user.PhoneNumber!,
                    code);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return ResponseModel<bool>.Success(true);
    }

    private async Task<ResponseModel<bool>> SendPasswordResetEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
            return ResponseModel<bool>.Failure("User not found.");
        var code = await GenerateVerificationCodeAsync();
        user.ResetPasswordCode = code;
        user.ResetPasswordExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendPasswordResetEmailAsync(user.Email!,
            code);
        return ResponseModel<bool>.Success(true);
    }

    private async Task<AppUser?> FindUserByEmailOrUserNameAsync(string? email, string? userName)
    {
        AppUser? user = null;
        if (!string.IsNullOrEmpty(email))
        {
            user = await userManager.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        else if (!string.IsNullOrEmpty(userName))
        {
            user = await userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        return user;
    }

    private async Task<AppUser?> FindUserByPhoneNumberAsync(string phoneNumber)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        return user;
    }

    private static async Task<string> GenerateVerificationCodeAsync() => await Task.FromResult(new Random().Next(100000, 999999).ToString());
}