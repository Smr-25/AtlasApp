using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Application.Settings;
using Atlas.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Atlas.Infrastructure.Services;

public class JwtService(IOptions<JwtSettings> jwtOptions) : IJwtService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public AccessTokenResponseDto GenerateAccessToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
            new("FullName", user.FullName)
        };

        if (!string.IsNullOrEmpty(user.PhoneNumber))
            claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        var accessToken = tokenHandler.WriteToken(token);
        var expiresAt = _jwtSettings.AccessTokenExpirationMinutes;
        var accessTokenResponse = new AccessTokenResponseDto
        (
            Token: accessToken,
            Expiration: DateTime.UtcNow.AddMinutes(expiresAt)
        );
        return accessTokenResponse;
    }

    public RefreshTokenResponseDto GenerateRefreshTokenResponse(AppUser user)
    {
        var refreshTokenResponse = new RefreshTokenResponseDto
        (
            RefreshToken: Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            RefreshTokenExpiresAt: DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        );
        return refreshTokenResponse;
    }
}