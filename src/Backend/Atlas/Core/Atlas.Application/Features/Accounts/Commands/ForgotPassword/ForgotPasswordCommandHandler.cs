using Atlas.Application.Common.Models;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler(UserManager<AppUser> userManager, IEmailService emailService)
    : IRequestHandler<ForgotPasswordCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return ResponseModel<bool>.Success(true);
        var code = GenerateVerificationCode();
        user.ResetPasswordCode = code;
        user.ResetPasswordExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendPasswordResetEmailAsync(user.Email!, code);
        return ResponseModel<bool>.Success(true);
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