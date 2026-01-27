using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.SetTelegramChatId;

public class SetTelegramChatIdCommandHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
    : IRequestHandler<SetTelegramChatIdCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(SetTelegramChatIdCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException("User is not authenticated");

        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null)
            throw new NotFoundException(nameof(AppUser));


        user.TelegramChatId = request.TelegramChatId;
        await userManager.UpdateAsync(user);

        return ResponseModel<bool>.Success(true);
    }
}