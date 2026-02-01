using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Helpers;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.ResendEmailVerification;

public class ResendEmailVerificationCommandHandler(UserManager<AppUser> userManager, IEmailService emailService)
    : IRequestHandler<ResendEmailVerificationCommand, bool>
{
    public async Task<bool> Handle(ResendEmailVerificationCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException("User", request.Email);

        if (user.EmailConfirmed)
            throw new AlreadyVerifiedException("Email");

        await SendEmailVerificationCodeAsync(user);
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