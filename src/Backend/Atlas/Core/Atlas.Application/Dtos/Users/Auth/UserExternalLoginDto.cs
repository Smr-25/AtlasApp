namespace Atlas.Application.Dtos.Users.Auth;

public record UserExternalLoginDto
(
    string Provider,
    string IdToken,
    string? AccessToken = null,
    string? AuthorizationCode = null
);