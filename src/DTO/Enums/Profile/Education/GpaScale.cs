using DTO.Attributes;

namespace DTO.Enums.Profile.Education;

public enum GpaScale
{
    [LocalizationKey("enum.gpaScale.fourPoint")]
    FourPoint = 1,
    [LocalizationKey("enum.gpaScale.fivePoint")]
    FivePoint = 2,
    [LocalizationKey("enum.gpaScale.tenPoint")]
    TenPoint = 3,
    [LocalizationKey("enum.gpaScale.other")]
    Other = 4
}
