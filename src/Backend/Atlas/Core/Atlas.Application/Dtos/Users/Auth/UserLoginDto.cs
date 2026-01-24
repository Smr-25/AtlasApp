namespace Atlas.Application.Dtos.Users.Auth;

public record UserLoginDto(
    string? UserName,
    string? Email,
    string Password
);