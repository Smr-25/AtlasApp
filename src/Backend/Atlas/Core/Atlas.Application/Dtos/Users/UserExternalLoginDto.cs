namespace Atlas.Application.Dtos.Users;

public record UserExternalLoginDto(
    string Provider,
    string IdToken
);