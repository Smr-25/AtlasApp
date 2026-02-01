using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.RefreshToken;

public class RefreshTokenCommandHandler(UserManager<AppUser> userManager, IJwtService jwtService)
    : IRequestHandler<RefreshTokenCommand, TokenDto>
{
    public async Task<TokenDto> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u =>
            u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshToken != request.RefreshToken ||
            user.RefreshTokenExpiresAt < DateTime.UtcNow)
            throw new InvalidCredentialsException("Invalid or expired refresh token.");

        if (user.IsDeleted)
            throw new UnauthorizedException("This account has been deleted.");

        if (user.IsLockedOut)
            throw new AccountLockedException("Account is locked. Token refresh is not allowed.");

        var newAccessToken = jwtService.GenerateAccessToken(user);
        var newRefreshToken = jwtService.GenerateRefreshTokenResponse(user);

        user.SetRefreshToken(newRefreshToken.RefreshToken, newRefreshToken.RefreshTokenExpiresAt);
        await userManager.UpdateAsync(user);

        var tokenDto = new TokenDto
        (
            newAccessToken.Token,
            newRefreshToken.RefreshToken,
            newAccessToken.Expiration,
            newRefreshToken.RefreshTokenExpiresAt
        );
        return tokenDto;
    }
}