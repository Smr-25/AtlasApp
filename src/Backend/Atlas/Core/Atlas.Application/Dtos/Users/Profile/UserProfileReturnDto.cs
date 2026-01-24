namespace Atlas.Application.Dtos.Users.Profile;

public record UserProfileReturnDto(
    string Id,
    string FullName,
    string UserName,
    string Email,
    string? PhoneNumber,
    bool EmailConfirmed,
    bool PhoneNumberConfirmed,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);