using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.ResetPassword;

public class ResetPasswordCommandHandler(UserManager<AppUser> userManager)
    : IRequestHandler<ResetPasswordCommand, bool>
{
    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException("User", request.Email);
        
        if (user.ResetPasswordCode != request.VerificationCode ||
            user.ResetPasswordExpiresAt < DateTime.UtcNow)
            throw new InvalidOrExpiredCodeException("Password Reset");
        
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description));

        user.ResetPasswordCode = null;
        user.ResetPasswordExpiresAt = null;
        await userManager.UpdateAsync(user);

        return true;
    }
}