using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Models;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.VerifyPhone;

public class VerifyPhoneCommandHandler(UserManager<AppUser> userManager)
    : IRequestHandler<VerifyPhoneCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(VerifyPhoneCommand request, CancellationToken cancellationToken)
    {
        var user = await FindUserByPhoneNumberAsync(request.PhoneNumber);

        if (user == null)
            throw new NotFoundException("User", request.PhoneNumber);

        if (user.PhoneVerificationCode != request.VerificationCode ||
            user.PhoneVerificationExpiresAt < DateTime.UtcNow)
            throw new InvalidOrExpiredCodeException("Phone Verification");

        user.PhoneNumberConfirmed = true;
        user.PhoneVerificationCode = null;
        user.PhoneVerificationExpiresAt = null;

        if (user.EmailConfirmed)
            user.Activate();

        await userManager.UpdateAsync(user);
        return ResponseModel<bool>.Success(true);
    }

    private async Task<AppUser?> FindUserByPhoneNumberAsync(string phoneNumber)
    {
        return await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }
}