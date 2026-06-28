using DTO.Attributes;

namespace DTO.Enums.Subscription;

public enum SubscriptionPlan
{
    [LocalizationKey("enum.subscriptionPlan.free")]
    Free = 1,
    [LocalizationKey("enum.subscriptionPlan.pro")]
    Pro = 2,
    [LocalizationKey("enum.subscriptionPlan.premium")]
    Premium = 3
}
