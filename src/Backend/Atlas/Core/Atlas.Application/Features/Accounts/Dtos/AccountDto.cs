using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Accounts.Dtos;

public record AccountDto(
    string Id,
    string? UserName,
    string Email,
    string FullName,
    string? PhoneNumber,
    bool EmailConfirmed,
    bool PhoneNumberConfirmed,
    DateTime CreatedAt,
    UserStatus Status,
    DateTime? LastLoginAt,
    string? Bio = null,
    IReadOnlyCollection<string>? Tags = null
);