using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Models;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.ResendEmailVerification;

public class ResendEmailVerificationCommandHandler(UserManager<AppUser> userManager, IEmailService emailService)
    : IRequestHandler<ResendEmailVerificationCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(ResendEmailVerificationCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException("User", request.Email);

        if (user.EmailConfirmed)
            throw new AlreadyVerifiedException("Email");

        await SendEmailVerificationCodeAsync(user);
        return ResponseModel<bool>.Success(true);
    }

    private async Task SendEmailVerificationCodeAsync(AppUser user)
    {
        var code = GenerateVerificationCode();
        user.EmailVerificationCode = code;
        user.EmailVerificationExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendVerificationEmailAsync(user.Email!, code);
    }

    private static string GenerateVerificationCode()
    {
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var code = (BitConverter.ToUInt32(bytes, 0) % 900000 + 100000).ToString();
        return code;
    }
}