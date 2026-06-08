using DTO.Attributes;

namespace DTO.Enums.Profile.UserProfile;

public enum LocationMode
{
    [LocalizationKey("enum.locationMode.specific")]
    Specific = 1,
    [LocalizationKey("enum.locationMode.anywhereInUs")]
    AnywhereInUs = 2,
    [LocalizationKey("enum.locationMode.remote")]
    Remote = 3
}
