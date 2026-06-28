using DTO.Enums.Subscription;

namespace DTO.Admin;

public sealed record AdminSubscriptionListItemResponse(
    int UserId,
    string UserEmail,
    SubscriptionPlan Plan,
    SubscriptionStatus Status,
    BillingInterval? BillingInterval,
    DateTime? StartDate,
    DateTime? CurrentPeriodEnd,
    DateTime? CanceledAt,
    string? StripeSubscriptionId
);
