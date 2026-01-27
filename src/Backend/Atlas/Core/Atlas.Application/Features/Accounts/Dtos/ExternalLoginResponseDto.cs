namespace Atlas.Application.Features.Accounts.Dtos;

public record ExternalLoginResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiration,
    bool IsNewUser,
    string UserId,
    string Email,
    string FullName
);