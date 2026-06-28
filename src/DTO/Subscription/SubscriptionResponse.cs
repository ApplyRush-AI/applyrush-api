using DTO.Enums.Subscription;

namespace DTO.Subscription;

public sealed record SubscriptionResponse(
    SubscriptionPlan Plan,
    SubscriptionStatus Status,
    BillingInterval? BillingInterval,
    DateTime? RenewalDate,
    string Price,
    DateTime? CanceledAt
);
