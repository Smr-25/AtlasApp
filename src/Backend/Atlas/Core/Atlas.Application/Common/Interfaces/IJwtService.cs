using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;

namespace Atlas.Application.Common.Interfaces;

public interface IJwtService
{
    AccessTokenResponseDto GenerateAccessToken(AppUser user); 
    RefreshTokenResponseDto GenerateRefreshTokenResponse(AppUser user);
    
}