using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class Subscription : BaseEntity
{
    public Guid UserId { get; private set; }
    public SubscriptionTier Tier { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public string? StripeCustomerId { get; private set; }
    public string? StripeSubscriptionId { get; private set; }
    public DateTime? CurrentPeriodEnd { get; private set; }
    public int MaxWorkspaces { get; private set; }
    public int MaxIntegrations { get; private set; }
    public bool HasCustomHotkeys { get; private set; }

    private Subscription() { }

    public static Subscription CreateFree(Guid userId)
    {
        return new Subscription
        {
            UserId = userId,
            Tier = SubscriptionTier.Free,
            Status = SubscriptionStatus.Active,
            MaxWorkspaces = 3,
            MaxIntegrations = 3,
            HasCustomHotkeys = false
        };
    }

    public static Subscription CreatePro(Guid userId, string stripeCustomerId, string stripeSubscriptionId, DateTime periodEnd)
    {
        return new Subscription
        {
            UserId = userId,
            Tier = SubscriptionTier.Pro,
            Status = SubscriptionStatus.Active,
            StripeCustomerId = stripeCustomerId,
            StripeSubscriptionId = stripeSubscriptionId,
            CurrentPeriodEnd = periodEnd,
            MaxWorkspaces = int.MaxValue,
            MaxIntegrations = int.MaxValue,
            HasCustomHotkeys = true
        };
    }

    public static Subscription CreateTeam(Guid userId, string stripeCustomerId, string stripeSubscriptionId, DateTime periodEnd)
    {
        return new Subscription
        {
            UserId = userId,
            Tier = SubscriptionTier.Team,
            Status = SubscriptionStatus.Active,
            StripeCustomerId = stripeCustomerId,
            StripeSubscriptionId = stripeSubscriptionId,
            CurrentPeriodEnd = periodEnd,
            MaxWorkspaces = int.MaxValue,
            MaxIntegrations = int.MaxValue,
            HasCustomHotkeys = true
        };
    }

    public void UpgradeTo(SubscriptionTier tier, string stripeCustomerId, string stripeSubscriptionId, DateTime periodEnd)
    {
        Tier = tier;
        StripeCustomerId = stripeCustomerId;
        StripeSubscriptionId = stripeSubscriptionId;
        CurrentPeriodEnd = periodEnd;
        Status = SubscriptionStatus.Active;

        switch (tier)
        {
            case SubscriptionTier.Pro:
                MaxWorkspaces = int.MaxValue;
                MaxIntegrations = int.MaxValue;
                HasCustomHotkeys = true;
                break;
            case SubscriptionTier.Team:
                MaxWorkspaces = int.MaxValue;
                MaxIntegrations = int.MaxValue;
                HasCustomHotkeys = true;
                break;
            default:
                MaxWorkspaces = 3;
                MaxIntegrations = 3;
                HasCustomHotkeys = false;
                break;
        }

        SetModified();
    }

    public void Cancel()
    {
        Status = SubscriptionStatus.Canceled;
        SetModified();
    }

    public void MarkPastDue()
    {
        Status = SubscriptionStatus.PastDue;
        SetModified();
    }

    public void Renew(DateTime newPeriodEnd)
    {
        Status = SubscriptionStatus.Active;
        CurrentPeriodEnd = newPeriodEnd;
        SetModified();
    }

    public void UpdateStripeInfo(string stripeCustomerId, string stripeSubscriptionId)
    {
        StripeCustomerId = stripeCustomerId;
        StripeSubscriptionId = stripeSubscriptionId;
        SetModified();
    }

    public bool IsActive => Status == SubscriptionStatus.Active || Status == SubscriptionStatus.Trialing;
}

