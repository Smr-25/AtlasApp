using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Models;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.Register;

public class RegisterCommandHandler(
    UserManager<AppUser> userManager,
    IEmailService emailService,
    ISmsService smsService,
    ITelegramService telegramService
) : IRequestHandler<RegisterCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByNameAsync(request.UserName);
        if (existingUser != null)
            throw new AlreadyExistException("User", request.UserName);
        
        var existingEmail = await userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
            throw new AlreadyExistException("Email", request.Email);
        
        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            var existingPhone = await userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);
            if (existingPhone != null)
                throw new AlreadyExistException("PhoneNumber", request.PhoneNumber);
        }

        var user = AppUser.Create(
            request.FullName,
            request.Email,
            request.FullName,
            request.PhoneNumber,
            request.PhoneVerificationChannel
        );
        
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description).ToArray());
        
        await SendEmailVerificationCodeAsync(user);
        
        if (!string.IsNullOrEmpty(request.PhoneNumber) && request.PhoneVerificationChannel.HasValue)
            await SendPhoneVerificationCodeAsync(user, request.PhoneVerificationChannel.Value);
        
        return ResponseModel<bool>.Success(true);
    }

    private async Task SendEmailVerificationCodeAsync(AppUser user)
    {
        var code = GenerateVerificationCode();
        user.EmailVerificationCode = code;
        user.EmailVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendVerificationEmailAsync(user.Email!, code);
    }
    
    private async Task SendPhoneVerificationCodeAsync(AppUser user, UserVerificationChannel requestPhoneVerificationChannel)
    {
        var code = GenerateVerificationCode();
        user.PhoneVerificationCode = code;
        user.PhoneVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);

        switch (requestPhoneVerificationChannel)
        {
            case UserVerificationChannel.Sms:
                await smsService.SendVerificationSmsAsync(user.PhoneNumber!, code);
                break;
            case UserVerificationChannel.Telegram:
                if (!string.IsNullOrEmpty(user.TelegramChatId))
                    await telegramService.SendVerificationCodeAsync(user.TelegramChatId, code);
                break;
            default:
                throw new InvalidVerificationChannelException("The selected phone verification channel is invalid.");
        }
    }
    
    private static string GenerateVerificationCode()
    {
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var code = (BitConverter.ToUInt32(bytes, 0) % 900000 + 100000).ToString();
        return code;
    }
}