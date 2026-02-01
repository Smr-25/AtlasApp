using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.GenerateTelegramLinkCode;

public class GenerateTelegramLinkCodeCommandHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
    : IRequestHandler<GenerateTelegramLinkCodeCommand, string>
{
    public async Task<string> Handle(GenerateTelegramLinkCodeCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException("User is not authenticated");
            
        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null)
            throw new NotFoundException("User", currentUserService.UserId!);

        var linkCode = Guid.NewGuid().ToString("N")[..8].ToUpper();

        user.TelegramLinkCode = linkCode;
        user.TelegramLinkCodeExpiry = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        return linkCode;
    }
}