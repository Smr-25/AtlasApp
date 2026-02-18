using Atlas.Application.Common.Helpers;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Accounts.Commands.LinkTelegramByChatId;

public class LinkTelegramByChatIdCommandHandler(
    UserManager<AppUser> userManager,
    ITelegramService telegramService,
    ILogger<LinkTelegramByChatIdCommandHandler> logger)
    : IRequestHandler<LinkTelegramByChatIdCommand, Unit>
{
    public async Task<Unit> Handle(LinkTelegramByChatIdCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u =>
            u.TelegramLinkCode == request.LinkCode &&
            u.TelegramLinkCodeExpiry > DateTime.UtcNow, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("Invalid or expired Telegram link code: {LinkCode}, ChatId: {ChatId}", 
                request.LinkCode, request.ChatId);
            return Unit.Value;
        }

        // Link Telegram ChatId
        user.TelegramChatId = request.ChatId;
        user.TelegramLinkCode = null;
        user.TelegramLinkCodeExpiry = null;
        
        // If phone verification is pending and user prefers Telegram, send verification code
        if (!user.PhoneNumberConfirmed && 
            !string.IsNullOrEmpty(user.PhoneNumber) &&
            user.PreferredVerificationChannel == UserVerificationChannel.Telegram)
        {
            var code = VerificationCodeGenerator.Generate();
            user.PhoneVerificationCode = code;
            user.PhoneVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
            
            await userManager.UpdateAsync(user);
            
            // Send verification code via Telegram
            await telegramService.SendVerificationCodeAsync(request.ChatId, code);
            
            logger.LogInformation("Telegram linked and verification code sent for user {UserId}", user.Id);
        }
        else
        {
            await userManager.UpdateAsync(user);
            
            // Send welcome message
            await telegramService.SendMessageAsync(request.ChatId, 
                "✅ Telegram hesabınız uğurla bağlandı! Artıq bildirişləri buradan alacaqsınız.");
            
            logger.LogInformation("Telegram linked successfully for user {UserId} with ChatId {ChatId}", 
                user.Id, request.ChatId);
        }
        
        return Unit.Value;
    }
}