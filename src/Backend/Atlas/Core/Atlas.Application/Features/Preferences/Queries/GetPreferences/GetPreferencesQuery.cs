using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Preferences.Dtos;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Preferences.Queries.GetPreferences;

public record GetPreferencesQuery : IRequest<UserPreferenceDto>;

public class GetPreferencesQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<GetPreferencesQuery, UserPreferenceDto>
{
    public async Task<UserPreferenceDto> Handle(GetPreferencesQuery request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();

        var pref = await context.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

        if (pref == null)
        {
            pref = UserPreference.CreateDefault(userId);
            await context.UserPreferences.AddAsync(pref, ct);
            await context.SaveChangesAsync(ct);
        }

        return new UserPreferenceDto(
            pref.Language, pref.Theme, pref.Timezone,
            pref.EmailNotifications, pref.PushNotifications,
            pref.InboxAlerts, pref.InboxApprovals, pref.InboxMentions, pref.InboxSystem,
            pref.WeeklyDigest, pref.CustomSettingsJson);
    }
}

