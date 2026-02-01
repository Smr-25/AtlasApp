using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Commands.AddPhoneNumber;

public class AddPhoneNumberCommandHandler(
    UserManager<AppUser> userManager,
    IPhoneVerificationService phoneVerificationService,
    ICurrentUserService currentUserService) : IRequestHandler<AddPhoneNumberCommand, bool>
{
    public async Task<bool> Handle(AddPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException("User is not authenticated");
            
        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null)
            throw new NotFoundException("User", currentUserService.UserId!);
        
        if (!string.IsNullOrEmpty(user.PhoneNumber))
            throw new BadRequestException("Phone number already exists. Use update phone number instead.");

        var existingPhone = await userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (existingPhone != null)
            throw new AlreadyExistException("PhoneNumber", request.PhoneNumber);

        user.PhoneNumber = request.PhoneNumber;
        user.PreferredVerificationChannel = request.VerificationChannel;
        await userManager.UpdateAsync(user);

        await phoneVerificationService.SendVerificationCodeAsync(user, request.VerificationChannel);

        return true;
    }
}