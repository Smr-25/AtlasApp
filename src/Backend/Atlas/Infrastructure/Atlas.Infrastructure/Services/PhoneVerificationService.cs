using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;

namespace Atlas.Infrastructure.Services;

public class PhoneVerificationService : IPhoneVerificationService
{
    public Task SendVerificationCodeAsync(AppUser user, UserVerificationChannel channel)
    {
        
    }
}