using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Queries.GetProfile;

public class GetProfileQueryHandler(
    UserManager<AppUser> userManager, 
    IMapper mapper, 
    ICurrentUserService currentUserService) : IRequestHandler<GetProfileQuery, AccountDto>
{
    public async Task<AccountDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException("User is not authenticated");

        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        return user == null ? throw new NotFoundException(nameof(AppUser)) : mapper.Map<AccountDto>(user);
    }
}