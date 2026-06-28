using DTO.Attributes;

namespace DTO.Enums.Subscription;

public enum BillingInterval
{
    [LocalizationKey("enum.billingInterval.monthly")]
    Monthly = 1,
    [LocalizationKey("enum.billingInterval.quarterly")]
    Quarterly = 2
}
