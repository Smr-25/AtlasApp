using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Domain.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = null!;
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
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEndTime { get; set; }
    public bool IsLockedOut  => LockoutEndTime.HasValue && LockoutEndTime > DateTime.UtcNow;

    public static AppUser Create(string userName, string email, string fullName, string? phoneNumber = null, 
        UserVerificationChannel? preferredChannel = null)
    {
        return new AppUser
        {
            UserName = userName,
            Email = email,
            FullName = fullName,
            PhoneNumber = phoneNumber,
            PreferredVerificationChannel = preferredChannel,
            Status = UserStatus.PendingVerification,
            CreatedAt = DateTime.UtcNow
        };
    }
    public void Activate()
    {
        Status = UserStatus.Active;
        ActivatedAt = DateTime.UtcNow;
    }
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string? fullName, string? userName)
    {
        if (!string.IsNullOrEmpty(fullName))
            FullName = fullName;
        if (!string.IsNullOrEmpty(userName))
            UserName = userName;
    }

    public void SetRefreshToken(string token, DateTime expiresAt)
    {
        RefreshToken = token;
        RefreshTokenExpiresAt = expiresAt;
    }
    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresAt = null;
    }
    public void MarkAsDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
    
    public void IncrementFailedLoginAttempts(int maxAttempts, TimeSpan lockoutDuration)
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= maxAttempts)
        {
            LockoutEndTime = DateTime.UtcNow.Add(lockoutDuration);
        }
    }

    public void ResetFailedLoginAttempts()
    {
        FailedLoginAttempts = 0;
        LockoutEndTime = null;
    }
}