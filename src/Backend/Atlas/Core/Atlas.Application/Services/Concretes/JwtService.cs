using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Atlas.Application.Dtos.Users.Auth;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Atlas.Application.Services.Concretes;

public class JwtService(IConfiguration configuration) : IJwtService
{
    public string GenerateAccessToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
            new("FullName", user.FullName)
        };

        if (!string.IsNullOrEmpty(user.PhoneNumber))
            claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration.GetSection("JwtSettings:SecretKey").Value!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expireMinutes = int.Parse(configuration.GetSection("JwtSettings:AccessTokenExpirationMinutes").Value!);

        var token = new JwtSecurityToken(
            issuer: configuration.GetSection("JwtSettings:Issuer").Value,
            audience: configuration.GetSection("JwtSettings:Audience").Value,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public UserRefreshTokenResponseDto GenerateRefreshTokenResponse(AppUser user)
    {
        var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var refreshTokenExpireDays =
            int.Parse(configuration.GetSection("JwtSettings:RefreshTokenExpirationDays").Value!);
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpireDays);

        var accessToken = GenerateAccessToken(user);

        return new UserRefreshTokenResponseDto(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            RefreshTokenExpiresAt: refreshTokenExpiresAt
        );
    }
}