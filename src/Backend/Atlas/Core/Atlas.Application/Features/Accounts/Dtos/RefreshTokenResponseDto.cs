namespace Atlas.Application.Features.Accounts.Dtos;

public record RefreshTokenResponseDto(
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);