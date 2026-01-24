namespace Atlas.Application.Dtos.Users.ExternalAuth;

public record UserExternalLoginDto(
    string Provider,
    string IdToken
);