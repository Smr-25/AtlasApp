namespace Atlas.Application.Dtos.Users;

public record UserForgotPasswordDto
(
    string? Email,
    string? PhoneNumber
);