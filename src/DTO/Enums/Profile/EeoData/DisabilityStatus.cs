using DTO.Attributes;

namespace DTO.Enums.Profile.EeoData;

public enum DisabilityStatus
{
    [LocalizationKey("enum.disabilityStatus.no")]
    No = 1,
    [LocalizationKey("enum.disabilityStatus.yes")]
    Yes = 2,
    [LocalizationKey("enum.disabilityStatus.preferNotToSay")]
    PreferNotToSay = 3
}
