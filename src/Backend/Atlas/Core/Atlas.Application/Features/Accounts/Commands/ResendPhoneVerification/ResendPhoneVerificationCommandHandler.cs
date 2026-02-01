using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.ResendPhoneVerification;

public class ResendPhoneVerificationCommandHandler(
    UserManager<AppUser> userManager, 
    IPhoneVerificationService phoneVerificationService) 
    : IRequestHandler<ResendPhoneVerificationCommand, bool>
{
    public async Task<bool> Handle(ResendPhoneVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);
        if (user == null)
            throw new NotFoundException("User", request.PhoneNumber);

        if (user.PhoneNumberConfirmed)
            throw new AlreadyVerifiedException("Phone Number");

        await phoneVerificationService.SendVerificationCodeAsync(user, request.Channel);
        return true;
    }
}