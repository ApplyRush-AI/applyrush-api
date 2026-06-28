using DTO.Enums.Subscription;

namespace Domain.Entities.Subscriptions.UserSubscriptions;

public interface IUserSubscriptionUpdateData
{
    string? StripeSubscriptionId { get; }
    SubscriptionPlan Plan { get; }
    BillingInterval? BillingInterval { get; }
    SubscriptionStatus Status { get; }
    DateTime? CurrentPeriodStart { get; }
    DateTime? CurrentPeriodEnd { get; }
    DateTime? CanceledAt { get; }
}
