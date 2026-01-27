using Atlas.Application.Common.Helpers;
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
        var code = VerificationCodeGenerator.Generate();
        user.ResetPasswordCode = code;
        user.ResetPasswordExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await userManager.UpdateAsync(user);
        await emailService.SendPasswordResetEmailAsync(user.Email!, code);
        return ResponseModel<bool>.Success(true);
    }
}