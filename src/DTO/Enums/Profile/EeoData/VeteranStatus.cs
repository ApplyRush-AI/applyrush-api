using DTO.Attributes;

namespace DTO.Enums.Profile.EeoData;

public enum VeteranStatus
{
    [LocalizationKey("enum.veteranStatus.notVeteran")]
    NotVeteran = 1,
    [LocalizationKey("enum.veteranStatus.veteran")]
    Veteran = 2,
    [LocalizationKey("enum.veteranStatus.activeDuty")]
    ActiveDuty = 3,
    [LocalizationKey("enum.veteranStatus.reservist")]
    Reservist = 4
}
