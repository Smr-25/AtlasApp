using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Preferences.Dtos;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Preferences.Commands.UpdatePreferences;

public record UpdatePreferencesCommand(
    string? Language = null,
    string? Theme = null,
    string? Timezone = null,
    bool? EmailNotifications = null,
    bool? PushNotifications = null,
    bool? InboxAlerts = null,
    bool? InboxApprovals = null,
    bool? InboxMentions = null,
    bool? InboxSystem = null,
    bool? WeeklyDigest = null,
    string? CustomSettingsJson = null
) : IRequest<UserPreferenceDto>;

public class UpdatePreferencesCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<UpdatePreferencesCommand, UserPreferenceDto>
{
    public async Task<UserPreferenceDto> Handle(UpdatePreferencesCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();

        var pref = await context.UserPreferences.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        if (pref == null)
        {
            pref = UserPreference.CreateDefault(userId);
            await context.UserPreferences.AddAsync(pref, ct);
        }

        pref.Update(
            request.Language, request.Theme, request.Timezone,
            request.EmailNotifications, request.PushNotifications,
            request.InboxAlerts, request.InboxApprovals, request.InboxMentions, request.InboxSystem,
            request.WeeklyDigest, request.CustomSettingsJson);

        await context.SaveChangesAsync(ct);

        return new UserPreferenceDto(
            pref.Language, pref.Theme, pref.Timezone,
            pref.EmailNotifications, pref.PushNotifications,
            pref.InboxAlerts, pref.InboxApprovals, pref.InboxMentions, pref.InboxSystem,
            pref.WeeklyDigest, pref.CustomSettingsJson);
    }
}

