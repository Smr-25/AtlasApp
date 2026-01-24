namespace Atlas.Application.Dtos.Users.Auth;

public record UserExternalLoginReturnDto
(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiryTime,
    bool IsNewUser,
    string UserId,
    string Email,
    string FullName
);