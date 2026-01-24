namespace Atlas.Application.Dtos.Users.Auth;

public record UserVerifyEmailDto
(
    string Email,
    string Code
);