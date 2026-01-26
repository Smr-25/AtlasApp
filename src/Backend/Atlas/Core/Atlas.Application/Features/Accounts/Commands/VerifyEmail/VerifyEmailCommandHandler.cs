using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Models;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.VerifyEmail;

public class VerifyEmailCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<VerifyEmailCommand,ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new NotFoundException("User", request.Email);

        if (user.EmailVerificationCode != request.VerificationCode ||
            user.EmailVerificationExpiresAt < DateTime.UtcNow)
            throw new InvalidOrExpiredCodeException("Email Verification");

        user.EmailConfirmed = true;
        user.EmailVerificationCode = null;
        user.EmailVerificationExpiresAt = null;

        if (string.IsNullOrEmpty(user.PhoneNumber) || user.PhoneNumberConfirmed)
            user.Activate();

        await userManager.UpdateAsync(user);
        return ResponseModel<bool>.Success(true);
    }
}