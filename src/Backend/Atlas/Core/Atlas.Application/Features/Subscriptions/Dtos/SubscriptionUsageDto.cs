namespace Atlas.Application.Features.Subscriptions.Dtos;

public record SubscriptionUsageDto(
    string Tier,
    string Status,
    int MaxWorkspaces,
    int CurrentWorkspaces,
    int MaxIntegrations,
    int CurrentIntegrations,
    bool HasCustomHotkeys,
    bool CanCreateWorkspace,
    bool CanAddIntegration,
    DateTime? CurrentPeriodEnd
);

