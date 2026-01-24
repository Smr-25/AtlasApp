using Atlas.Application.Dtos.Users.Auth;
using Atlas.Domain.Entities;

namespace Atlas.Application.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(AppUser user);
    UserRefreshTokenResponseDto GenerateRefreshTokenResponse(AppUser user);
}