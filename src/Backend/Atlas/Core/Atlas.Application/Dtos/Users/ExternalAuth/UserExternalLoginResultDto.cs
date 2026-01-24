namespace Atlas.Application.Dtos.Users.ExternalAuth;

public record UserExternalLoginResultDto(
    string AccessToken,
    string RefreshToken,
    bool IsNewUser
);