using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Accounts.Queries.GetProfile;

public class GetProfileQueryHandler(
    UserManager<AppUser> userManager,
    ICurrentUserService currentUserService,
    IMapper mapper
    ) : IRequestHandler<GetProfileQuery,ResponseModel<AccountDto>>
{
    public async Task<ResponseModel<AccountDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
    
        if (!currentUserService.IsAuthenticated || currentUserService.UserId == null)
            throw new UnauthorizedException("User is not authenticated");

        var user = await userManager.FindByIdAsync(currentUserService.UserId);
        if (user == null)
            throw new NotFoundException("User", currentUserService.UserId!);
       
        var profile = mapper.Map<AccountDto>(user);
        return ResponseModel<AccountDto>.Success(profile);
    }
}