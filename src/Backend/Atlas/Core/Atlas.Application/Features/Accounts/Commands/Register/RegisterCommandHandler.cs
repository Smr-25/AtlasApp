using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Helpers;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Application.Settings;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Atlas.Application.Features.Accounts.Commands.Register;

public class RegisterCommandHandler(
    UserManager<AppUser> userManager,
    IEmailService emailService,
    IPhoneVerificationService phoneVerificationService,
    IOptions<TelegramSettings> telegramSettings
) : IRequestHandler<RegisterCommand, RegisterResponseDto>
{
    private readonly TelegramSettings _telegramSettings = telegramSettings.Value;

    public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
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
            request.UserName,
            request.Email,
            request.FullName,
            request.PhoneNumber,
            request.PhoneVerificationChannel
        );
        
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description).ToArray());
        
        await SendEmailVerificationCodeAsync(user);

        string? telegramBotLink = null;
        var requiresPhoneVerification = !string.IsNullOrEmpty(request.PhoneNumber) && 
                                         request.PhoneVerificationChannel.HasValue;

        if (requiresPhoneVerification)
        {
            if (request.PhoneVerificationChannel == UserVerificationChannel.Telegram)
            {
                var linkCode = Guid.NewGuid().ToString("N")[..8].ToUpper();
                user.TelegramLinkCode = linkCode;
                user.TelegramLinkCodeExpiry = DateTime.UtcNow.AddMinutes(30);
                await userManager.UpdateAsync(user);
                
                telegramBotLink = $"https://t.me/{_telegramSettings.BotUsername}?start={linkCode}";
            }
            else
            {
                await phoneVerificationService.SendVerificationCodeAsync(user, request.PhoneVerificationChannel.Value);
            }
        }
        
        return new RegisterResponseDto(
            Success: true,
            RequiresEmailVerification: true,
            RequiresPhoneVerification: requiresPhoneVerification,
            TelegramBotLink: telegramBotLink
        );
    }

    private async Task SendEmailVerificationCodeAsync(AppUser user)
    {
        var code = VerificationCodeGenerator.Generate();
        user.EmailVerificationCode = code;
        user.EmailVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendVerificationEmailAsync(user.Email!, code);
    }
}