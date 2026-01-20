namespace Atlas.Application.Dtos.Users;

public record UserResetPasswordDto(
    string? Email,
    string? PhoneNumber,
    string Code,
    string NewPassword,
    string ConfirmPassword
);