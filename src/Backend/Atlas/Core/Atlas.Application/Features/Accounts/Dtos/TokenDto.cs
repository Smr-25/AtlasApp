namespace Atlas.Application.Features.Accounts.Dtos;

public record TokenDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiration,
    DateTime RefreshTokenExpiration
);