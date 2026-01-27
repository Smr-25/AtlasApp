using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Accounts.Commands.LinkTelegramByChatId;

public class LinkTelegramByChatIdCommandHandler(
    UserManager<AppUser> userManager,
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

        user.TelegramChatId = request.ChatId;
        user.TelegramLinkCode = null;
        user.TelegramLinkCodeExpiry = null;
        await userManager.UpdateAsync(user);
        
        logger.LogInformation("Telegram linked successfully for user {UserId} with ChatId {ChatId}", 
            user.Id, request.ChatId);
        
        return Unit.Value;
    }
}