using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Domain.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; private set; } = null!;
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ActivatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? EmailVerificationCode { get; set; }
    public DateTime? EmailVerificationExpiresAt { get; set; }
    public UserVerificationChannel? PreferredVerificationChannel { get; set; }
    public string? TelegramChatId { get; set; }
    public string? TelegramLinkCode { get; set; }
    public DateTime? TelegramLinkCodeExpiry { get; set; }
    public string? PhoneVerificationCode { get; set; }
    public DateTime? PhoneVerificationExpiresAt { get; set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }
    public string? ResetPasswordCode { get; set; }
    public DateTime? ResetPasswordExpiresAt { get; set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
}