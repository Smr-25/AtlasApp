namespace Atlas.Application.Dtos.Users.Auth;

public record UserRefreshTokenResponseDto(
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);