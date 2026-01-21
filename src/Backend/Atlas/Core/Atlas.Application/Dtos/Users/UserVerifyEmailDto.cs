namespace Atlas.Application.Dtos.Users;

public record UserVerifyEmailDto
(
    string Email,
    string Code
);