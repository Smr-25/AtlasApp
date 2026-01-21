namespace Atlas.Application.Dtos.Users;

public record UserForgotPasswordDto
(
    string? UserName,
    string? Email
);