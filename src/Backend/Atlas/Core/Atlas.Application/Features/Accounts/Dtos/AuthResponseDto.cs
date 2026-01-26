namespace Atlas.Application.Features.Accounts.Dtos;

public record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiration,
    DateTime RefreshTokenExpiration,
    string UserId,
    string UserName,
    string Email,
    string FullName
);