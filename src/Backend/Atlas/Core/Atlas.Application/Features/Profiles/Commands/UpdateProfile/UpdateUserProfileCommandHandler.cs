using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Profiles.Commands.UpdateProfile;

public class UpdateUserProfileCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateUserProfileCommand>
{
    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        var profile = await applicationDbContext.UserProfiles.FirstOrDefaultAsync(p => p.Id == userId, cancellationToken);
        if (profile == null) throw new NotFoundException("Profile", userId);

        profile.UpdateInfo(request.JobTitle, request.Bio);
        
        if (!string.IsNullOrEmpty(request.ThemeColor))
            profile.SetTheme(request.ThemeColor);

        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}