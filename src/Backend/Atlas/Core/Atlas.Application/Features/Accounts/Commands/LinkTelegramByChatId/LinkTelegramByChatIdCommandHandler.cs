using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.LinkTelegramByChatId;

public class LinkTelegramByChatIdCommandHandler(UserManager<AppUser> userManager)
    : IRequestHandler<LinkTelegramByChatIdCommand, Unit>
{
    public async Task<Unit> Handle(LinkTelegramByChatIdCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u =>
            u.TelegramLinkCode == request.LinkCode &&
            u.TelegramLinkCodeExpiry > DateTime.UtcNow);

        if (user == null)
            return new Unit();

        user.TelegramChatId = request.ChatId;
        user.TelegramLinkCode = null;
        user.TelegramLinkCodeExpiry = null;
        await userManager.UpdateAsync(user);
        return new Unit();
    }
}