namespace Atlas.Application.Dtos.Users;

public record UserResendCodeDto
(
    string? Email,
    string? PhoneNumber
);