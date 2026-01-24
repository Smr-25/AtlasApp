namespace Atlas.Application.Dtos.Users.Profile;

public record UserChangePasswordDto(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);