using Domain.Entities.Base;
using Domain.Entities.User;
using Domain.Events.Subscriptions;
using DTO.Enums.Subscription;

namespace Domain.Entities.Subscriptions.UserSubscriptions;

public sealed class UserSubscription : BaseAuditableEntity
{
    private UserSubscription() { }

    public int UserId { get; private set; }
    public string StripeCustomerId { get; private set; } = null!;
    public string? StripeSubscriptionId { get; private set; }
    public SubscriptionPlan Plan { get; private set; }
    public BillingInterval? BillingInterval { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime? CurrentPeriodStart { get; private set; }
    public DateTime? CurrentPeriodEnd { get; private set; }
    public DateTime? CanceledAt { get; private set; }

    public ApplicationUser User { get; } = null!;

    public static UserSubscription Create(IUserSubscriptionInsertData data)
    {
        var subscription = new UserSubscription
        {
            UserId = data.UserId,
            StripeCustomerId = data.StripeCustomerId,
            Plan = SubscriptionPlan.Free,
            Status = SubscriptionStatus.Active
        };

        subscription.AddDomainEvent(new SubscriptionCreatedEvent(subscription));
        return subscription;
    }

    public void Update(IUserSubscriptionUpdateData data)
    {
        StripeSubscriptionId = data.StripeSubscriptionId;
        Plan = data.Plan;
        BillingInterval = data.BillingInterval;
        Status = data.Status;
        CurrentPeriodStart = data.CurrentPeriodStart;
        CurrentPeriodEnd = data.CurrentPeriodEnd;
        CanceledAt = data.CanceledAt;

        AddDomainEvent(new SubscriptionUpdatedEvent(this));
    }

    public void Cancel(DateTime canceledAt)
    {
        CanceledAt = canceledAt;
        AddDomainEvent(new SubscriptionCanceledEvent(this));
    }
}
