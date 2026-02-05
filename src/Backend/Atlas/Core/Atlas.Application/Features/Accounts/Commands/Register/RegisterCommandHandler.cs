using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Helpers;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.Register;

public class RegisterCommandHandler(
    UserManager<AppUser> userManager,
    IEmailService emailService,
    IPhoneVerificationService phoneVerificationService
) : IRequestHandler<RegisterCommand, bool>
{
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByNameAsync(request.UserName);
        if (existingUser != null)
            throw new AlreadyExistException("User", request.UserName);
        
        var existingEmail = await userManager.FindByEmailAsync(request.Email);
        if (existingEmail != null)
            throw new AlreadyExistException("Email", request.Email);
        
        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            var existingPhone = await userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);
            if (existingPhone != null)
                throw new AlreadyExistException("PhoneNumber", request.PhoneNumber);
        }

        var user = AppUser.Create(
            request.UserName,
            request.Email,
            request.FullName,
            request.PhoneNumber,
            request.PhoneVerificationChannel
        );
        
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description).ToArray());
        
        await SendEmailVerificationCodeAsync(user);
        
        if (!string.IsNullOrEmpty(request.PhoneNumber) && request.PhoneVerificationChannel.HasValue)
            await phoneVerificationService.SendVerificationCodeAsync(user, request.PhoneVerificationChannel.Value);
        
        return true;
    }

    private async Task SendEmailVerificationCodeAsync(AppUser user)
    {
        var code = VerificationCodeGenerator.Generate();
        user.EmailVerificationCode = code;
        user.EmailVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendVerificationEmailAsync(user.Email!, code);
    }
}