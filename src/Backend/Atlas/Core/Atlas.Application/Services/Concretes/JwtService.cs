using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Atlas.Application.Models;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Atlas.Application.Services.Concretes;

public class JwtService(IConfiguration configuration) : IJwtService
{
    public string GenerateToken(AppUser user)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.MobilePhone, user.PhoneNumber!),
            new(ClaimTypes.Name, user.UserName!),
            new("FullName", $"{user.FullName}")
        ];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration.GetSection("JwtSettings:SecretKey").Value!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration.GetSection("JwtSettings:Issuer").Value,
            audience: configuration.GetSection("JwtSettings:Audience").Value,
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}