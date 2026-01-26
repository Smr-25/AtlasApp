using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.RevokeToken;

public class RevokeTokenCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<RevokeTokenCommand, Unit>
{
    public async Task<Unit> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
            throw new BadRequestException("Refresh token is required.");

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
        if (user == null)
            throw new InvalidCredentialsException("Invalid refresh token.");

        user.RevokeRefreshToken();
        await userManager.UpdateAsync(user);  
        return Unit.Value;
    }
}