using Atlas.Domain.Enums;

namespace Atlas.Domain.Events;

public class SubscriptionChangedEvent : DomainEventBase
{
    public Guid UserId { get; }
    public SubscriptionTier OldTier { get; }
    public SubscriptionTier NewTier { get; }

    public SubscriptionChangedEvent(Guid userId, SubscriptionTier oldTier, SubscriptionTier newTier)
    {
        UserId = userId;
        OldTier = oldTier;
        NewTier = newTier;
    }
}

