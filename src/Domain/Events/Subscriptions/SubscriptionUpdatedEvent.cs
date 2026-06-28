using Domain.Entities.Subscriptions.UserSubscriptions;

namespace Domain.Events.Subscriptions;

public sealed class SubscriptionUpdatedEvent : BaseEvent
{
    public SubscriptionUpdatedEvent(UserSubscription subscription)
    {
        Subscription = subscription;
    }

    public UserSubscription Subscription { get; }
}
