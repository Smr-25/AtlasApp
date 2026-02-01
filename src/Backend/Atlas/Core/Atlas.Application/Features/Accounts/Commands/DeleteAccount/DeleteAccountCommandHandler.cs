using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.DeleteAccount;

public class DeleteAccountCommandHandler(UserManager<AppUser> userManager, ICurrentUserService currentUserService)
    : IRequestHandler<DeleteAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException("User is not authenticated");

        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null)
            throw new NotFoundException(nameof(AppUser));


        user.MarkAsDeleted();
        await userManager.UpdateAsync(user);
        return true;
    }
}