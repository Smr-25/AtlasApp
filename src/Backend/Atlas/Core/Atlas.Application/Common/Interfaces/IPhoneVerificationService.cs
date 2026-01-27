using Atlas.Domain.Entities;
using Atlas.Domain.Enums;

namespace Atlas.Application.Common.Interfaces;

public interface IPhoneVerificationService
{
    Task SendVerificationCodeAsync(AppUser user, UserVerificationChannel channel);
}