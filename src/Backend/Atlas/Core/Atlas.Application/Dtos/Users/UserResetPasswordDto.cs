namespace Atlas.Application.Dtos.Users;

public record UserResetPasswordDto(
    string UserName,
    string? Email,
    string Code,
    string NewPassword,
    string ConfirmPassword
);