using Domain.Entities.Subscriptions.UserSubscriptions;

namespace Domain.Events.Subscriptions;

public sealed class SubscriptionCanceledEvent : BaseEvent
{
    public SubscriptionCanceledEvent(UserSubscription subscription)
    {
        Subscription = subscription;
    }

    public UserSubscription Subscription { get; }
}
