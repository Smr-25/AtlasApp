using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class UserPreference : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Language { get; private set; } = "en";
    public string Theme { get; private set; } = "system";
    public string Timezone { get; private set; } = "UTC";
    public bool EmailNotifications { get; private set; } = true;
    public bool PushNotifications { get; private set; } = true;
    public bool InboxAlerts { get; private set; } = true;
    public bool InboxApprovals { get; private set; } = true;
    public bool InboxMentions { get; private set; } = true;
    public bool InboxSystem { get; private set; } = true;
    public bool WeeklyDigest { get; private set; }
    public string? CustomSettingsJson { get; private set; }

    private UserPreference() { }

    public static UserPreference CreateDefault(Guid userId)
    {
        return new UserPreference { UserId = userId };
    }

    public void Update(
        string? language = null,
        string? theme = null,
        string? timezone = null,
        bool? emailNotifications = null,
        bool? pushNotifications = null,
        bool? inboxAlerts = null,
        bool? inboxApprovals = null,
        bool? inboxMentions = null,
        bool? inboxSystem = null,
        bool? weeklyDigest = null,
        string? customSettingsJson = null)
    {
        if (language != null) Language = language;
        if (theme != null) Theme = theme;
        if (timezone != null) Timezone = timezone;
        if (emailNotifications.HasValue) EmailNotifications = emailNotifications.Value;
        if (pushNotifications.HasValue) PushNotifications = pushNotifications.Value;
        if (inboxAlerts.HasValue) InboxAlerts = inboxAlerts.Value;
        if (inboxApprovals.HasValue) InboxApprovals = inboxApprovals.Value;
        if (inboxMentions.HasValue) InboxMentions = inboxMentions.Value;
        if (inboxSystem.HasValue) InboxSystem = inboxSystem.Value;
        if (weeklyDigest.HasValue) WeeklyDigest = weeklyDigest.Value;
        if (customSettingsJson != null) CustomSettingsJson = customSettingsJson;
        SetModified();
    }
}

