namespace Atlas.Application.Dtos.Users;

public record UserRegisterDto(
    string FullName,
    string UserName,
    string Email,
    string? PhoneNumber,
    string Password,
    string ConfirmPassword
);