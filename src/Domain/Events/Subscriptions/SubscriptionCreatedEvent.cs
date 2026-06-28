using Domain.Entities.Subscriptions.UserSubscriptions;

namespace Domain.Events.Subscriptions;

public sealed class SubscriptionCreatedEvent : BaseEvent
{
    public SubscriptionCreatedEvent(UserSubscription subscription)
    {
        Subscription = subscription;
    }

    public UserSubscription Subscription { get; }
}
