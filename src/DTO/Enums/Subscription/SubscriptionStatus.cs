using DTO.Attributes;

namespace DTO.Enums.Subscription;

public enum SubscriptionStatus
{
    [LocalizationKey("enum.subscriptionStatus.active")]
    Active = 1,
    [LocalizationKey("enum.subscriptionStatus.pastDue")]
    PastDue = 2,
    [LocalizationKey("enum.subscriptionStatus.canceled")]
    Canceled = 3,
    [LocalizationKey("enum.subscriptionStatus.trialing")]
    Trialing = 4,
    [LocalizationKey("enum.subscriptionStatus.incomplete")]
    Incomplete = 5
}
