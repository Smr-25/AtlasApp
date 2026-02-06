using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Profiles.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Profiles.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService) : IRequestHandler<GetUserProfileQuery, UserProfileDetailDto>
{
    public async Task<UserProfileDetailDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        var profile = await applicationDbContext.UserProfiles
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        if (profile == null)
            throw new NotFoundException("User Profile", userId);

        return new UserProfileDetailDto(
            profile.Id,
            profile.JobTitle,
            profile.Bio ?? string.Empty,
            profile.ThemeColor,
            profile.Profession.ToString(),
            profile.Workspaces 
        );
    }
}