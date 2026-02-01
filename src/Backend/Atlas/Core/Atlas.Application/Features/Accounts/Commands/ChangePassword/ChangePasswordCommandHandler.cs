using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.ChangePassword;

public class ChangePasswordCommandHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
    : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException("User is not authenticated");

        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null)
            throw new NotFoundException(nameof(AppUser));

        var passwordValid = await userManager.CheckPasswordAsync(user, request.CurrentPassword);
        if (!passwordValid)
            throw new InvalidCredentialsException("Current password is incorrect.");

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword,
            request.NewPassword);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description));

        return true;
    }
}