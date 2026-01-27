using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.RevokeToken;

public class RevokeAllTokenCommandHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
    : IRequestHandler<RevokeAllTokenCommand, Unit>
{
    public async Task<Unit> Handle(RevokeAllTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(currentUserService.UserId!);

        if (user == null)
            throw new NotFoundException("User", currentUserService.UserId!);

        user.RevokeAllRefreshTokens();
        await userManager.UpdateAsync(user);
        return Unit.Value;
    }
}