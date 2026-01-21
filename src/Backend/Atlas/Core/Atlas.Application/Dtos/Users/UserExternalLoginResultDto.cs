namespace Atlas.Application.Dtos.Users;

public record UserExternalLoginResultDto(
    string AccessToken,
    string RefreshToken,
    bool IsNewUser
);