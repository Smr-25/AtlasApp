using Atlas.Domain.Entities;

namespace Atlas.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(AppUser user);
    //UserRefreshTokenResponseDto GenerateRefreshTokenResponse(AppUser user);
}