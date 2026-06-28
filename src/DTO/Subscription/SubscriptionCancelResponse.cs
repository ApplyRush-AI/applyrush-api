namespace DTO.Subscription;

public sealed record SubscriptionCancelResponse(
    DateTime CancelledAt,
    DateTime? AccessUntil
);
