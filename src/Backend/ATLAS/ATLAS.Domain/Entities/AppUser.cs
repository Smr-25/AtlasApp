using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Domain.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; private set; } = null!;
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? EmailVerificationCode { get; private set; }
    public DateTime? EmailVerificationExpiresAt { get; private set; }
    public string? PhoneVerificationCode { get; private set; }
    public DateTime? PhoneVerificationExpiresAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
}