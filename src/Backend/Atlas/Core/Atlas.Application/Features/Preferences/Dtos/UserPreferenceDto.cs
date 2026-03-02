namespace Atlas.Application.Features.Preferences.Dtos;

public record UserPreferenceDto(
    string Language,
    string Theme,
    string Timezone,
    bool EmailNotifications,
    bool PushNotifications,
    bool InboxAlerts,
    bool InboxApprovals,
    bool InboxMentions,
    bool InboxSystem,
    bool WeeklyDigest,
    string? CustomSettingsJson
);

