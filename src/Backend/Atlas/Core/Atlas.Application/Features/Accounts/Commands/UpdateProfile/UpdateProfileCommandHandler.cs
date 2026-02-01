using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.UpdateProfile;

public class UpdateProfileCommandHandler(UserManager<AppUser> userManager, IMapper mapper, ICurrentUserService currentUserService)
    : IRequestHandler<UpdateProfileCommand, AccountDto>
{
    public async Task<AccountDto> Handle(UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException("User is not authenticated");

        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null)
            throw new NotFoundException(nameof(AppUser));


        if (!string.IsNullOrEmpty(request.UserName) && user.UserName != request.UserName)
        {
            var existingUserName = await userManager.FindByNameAsync(request.UserName);
            if (existingUserName != null)
                throw new AlreadyExistException("UserName", request.UserName);
        }

        user.UpdateProfile(request.FullName, request.UserName);

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new IdentityException(result.Errors.Select(e => e.Description));
        var accountDto = mapper.Map<AccountDto>(user);
        
        return accountDto;
    }
}