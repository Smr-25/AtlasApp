namespace Atlas.Application.Dtos.Users.Auth;

public record UserForgotPasswordDto
(
    string? UserName,
    string? Email
);