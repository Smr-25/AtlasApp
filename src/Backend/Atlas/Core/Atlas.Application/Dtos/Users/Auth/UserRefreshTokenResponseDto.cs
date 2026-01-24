namespace Atlas.Application.Dtos.Users.Auth;

public record UserRefreshTokenResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);