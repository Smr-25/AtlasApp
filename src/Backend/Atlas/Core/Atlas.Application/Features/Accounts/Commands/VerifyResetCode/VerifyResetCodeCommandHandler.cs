using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.VerifyResetCode;

public class VerifyResetCodeCommandHandler(UserManager<AppUser> userManager)
    : IRequestHandler<VerifyResetCodeCommand, VerifyResetCodeResponseDto>
{
    public async Task<VerifyResetCodeResponseDto> Handle(VerifyResetCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException("User", request.Email);

        if (user.ResetPasswordCode != request.VerificationCode ||
            user.ResetPasswordExpiresAt < DateTime.UtcNow)
            throw new InvalidOrExpiredCodeException("Password Reset");

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        
        user.ResetPasswordCode = null;
        user.ResetPasswordExpiresAt = null;
        await userManager.UpdateAsync(user);

        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        return new VerifyResetCodeResponseDto(resetToken, expiresAt);
    }
}

