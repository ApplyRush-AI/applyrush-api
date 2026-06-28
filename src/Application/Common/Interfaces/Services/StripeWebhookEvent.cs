using DTO.Enums.Subscription;

namespace Application.Common.Interfaces.Services;

public sealed record StripeWebhookEvent(
    string EventType,
    string? StripeSubscriptionId,
    string? StripeCustomerId,
    SubscriptionPlan? Plan,
    BillingInterval? Interval,
    SubscriptionStatus? Status,
    DateTime? CurrentPeriodStart,
    DateTime? CurrentPeriodEnd,
    DateTime? CanceledAt
);
