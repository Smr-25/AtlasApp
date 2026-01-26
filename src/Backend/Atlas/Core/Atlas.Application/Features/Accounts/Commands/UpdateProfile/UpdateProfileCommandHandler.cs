using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Commands.UpdateProfile;

public class UpdateProfileCommandHandler(UserManager<AppUser> userManager, IMapper mapper)
    : IRequestHandler<UpdateProfileCommand, ResponseModel<AccountDto>>
{
    public async Task<ResponseModel<AccountDto>> Handle(UpdateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            throw new NotFoundException("User", request.UserId);

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
        
        return ResponseModel<AccountDto>.Success(accountDto);
    }
}