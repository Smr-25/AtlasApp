using Atlas.Application.Dtos.Users;
using Atlas.Application.Interfaces;
using Atlas.Application.Models;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using AutoMapper;
using FluentValidation;
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
    IValidator<UserVerifyEmailDto> verifyAccountValidator,
    IJwtService jwtService,
    IEmailService emailService,
    ISmsService smsService
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

        var code = await GenerateVerificationCodeAsync();
        user.VerificationCode = code;
        user.VerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);

        await emailService.SendVerificationEmailAsync(user.Email!,
            code);

        if (!string.IsNullOrEmpty(userRegisterDto.PhoneNumber))
            await smsService.SendSmsAsync(user.PhoneNumber!,
                $"Your verification code is: {code}");


        return !result.Succeeded
            ? ResponseModel<bool>.Failure(result.Errors.Select(e => e.Description))
            : ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<UserLoginResponseDto>> LoginAsync(UserLoginDto userLoginDto)
    {
        var validationResult = await loginValidator.ValidateAsync(userLoginDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        var user = await userManager.FindByNameAsync(userLoginDto.UserName);
        if (user == null)
            return ResponseModel<UserLoginResponseDto>.Failure("Invalid username or password.");
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

    public async Task<ResponseModel<bool>> ForgotPasswordAsync(UserForgotPasswordDto userForgotPasswordDto)
    {
        var validationResult = await forgotPasswordValidator.ValidateAsync(userForgotPasswordDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await FindUserByEmailOrUserNameAsync(userForgotPasswordDto.Email,
            userForgotPasswordDto.UserName);

        if (user == null)
            return ResponseModel<bool>.Success(true);

        var code = new Random().Next(100000, 999999).ToString();
        user.ResetPasswordCode = code;
        user.ResetPasswordExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);

        await emailService.SendPasswordResetEmailAsync(user.Email!,
            $"Your password reset code is: {code}");
        return ResponseModel<bool>.Success(true);
    }

    public async Task<ResponseModel<bool>> ResetPasswordAsync(UserResetPasswordDto userResetPasswordDto)
    {
        var validationResult = await resetPasswordValidator.ValidateAsync(userResetPasswordDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await userManager.FindByEmailAsync(userResetPasswordDto.Email);

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

    public async Task<ResponseModel<bool>> VerifyAccountAsync(UserVerifyEmailDto userVerifyEmailDto)
    {
        var validationResult = await verifyAccountValidator.ValidateAsync(userVerifyEmailDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await userManager.FindByEmailAsync(userVerifyEmailDto.Email);

        if (user == null)
            return ResponseModel<bool>.Failure("User not found.");

        if (user.VerificationCode != userVerifyEmailDto.Code ||
            user.VerificationExpiresAt < DateTime.UtcNow)

            return ResponseModel<bool>.Failure("Invalid or expired verification code.");

        user.EmailConfirmed = true;
        user.VerificationCode = null;
        user.VerificationExpiresAt = null;
        await userManager.UpdateAsync(user);
        return ResponseModel<bool>.Success(true);
    }

    public Task<ResponseModel<bool>> ResendVerificationCodeAsync(UserVerifyEmailDto userVerifyEmailDto)
    {
        var validationResult = verifyAccountValidator.Validate(userVerifyEmailDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        throw new NotImplementedException();
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
    
    private static async Task<string> GenerateVerificationCodeAsync()
    {
        return await Task.FromResult(new Random().Next(100000, 999999).ToString());
    }
}