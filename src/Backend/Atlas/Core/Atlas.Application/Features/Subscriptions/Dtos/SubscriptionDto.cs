namespace Atlas.Application.Features.Subscriptions.Dtos;

public record SubscriptionDto(
    Guid Id,
    string Tier,
    string Status,
    int MaxWorkspaces,
    int MaxIntegrations,
    bool HasCustomHotkeys,
    DateTime? CurrentPeriodEnd
);

