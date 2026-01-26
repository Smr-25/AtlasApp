using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Models;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.SetTelegramChatId;

public class SetTelegramChatIdCommandHandler(UserManager<AppUser> userManager)
    : IRequestHandler<SetTelegramChatIdCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(SetTelegramChatIdCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            throw new NotFoundException("User", request.UserId);

        user.TelegramChatId = request.TelegramChatId;
        await userManager.UpdateAsync(user);

        return ResponseModel<bool>.Success(true);
    }
}