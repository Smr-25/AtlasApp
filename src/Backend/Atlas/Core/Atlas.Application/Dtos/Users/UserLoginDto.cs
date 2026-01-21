namespace Atlas.Application.Dtos.Users;

public record UserLoginDto(
    string UserName,
    string Email,
    string Password
);