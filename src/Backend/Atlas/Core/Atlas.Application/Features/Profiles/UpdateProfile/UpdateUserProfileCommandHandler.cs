using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Profiles.UpdateProfile;

public class UpdateUserProfileCommandHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<UpdateUserProfileCommand>
{
    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await applicationDbContext.UserProfiles.FirstOrDefaultAsync(p => p.Id == request.UserId, cancellationToken);
        if (profile == null) throw new NotFoundException("Profile", request.UserId);

        profile.UpdateInfo(request.JobTitle, request.Bio);
        
        if (!string.IsNullOrEmpty(request.ThemeColor))
            profile.SetTheme(request.ThemeColor);
        

        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}