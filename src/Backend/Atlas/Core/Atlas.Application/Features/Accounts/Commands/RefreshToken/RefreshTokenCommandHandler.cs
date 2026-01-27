using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.RefreshToken;

public class RefreshTokenCommandHandler(UserManager<AppUser> userManager, IJwtService jwtService)
    : IRequestHandler<RefreshTokenCommand, ResponseModel<TokenDto>>
{
    public async Task<ResponseModel<TokenDto>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u =>
            u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshToken != request.RefreshToken ||
            user.RefreshTokenExpiresAt < DateTime.UtcNow)
            throw new InvalidCredentialsException("Invalid refresh token.");

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
        return ResponseModel<TokenDto>.Success(tokenDto);
    }
}