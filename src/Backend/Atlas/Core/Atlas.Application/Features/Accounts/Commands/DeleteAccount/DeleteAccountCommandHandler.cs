using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Models;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.DeleteAccount;

public class DeleteAccountCommandHandler(UserManager<AppUser> userManager): IRequestHandler<DeleteAccountCommand, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            throw new NotFoundException("User", request.UserId);

        user.MarkAsDeleted();
        await userManager.UpdateAsync(user);
        return ResponseModel<bool>.Success(true);
    }
}