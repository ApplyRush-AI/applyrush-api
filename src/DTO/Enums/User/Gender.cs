using DTO.Attributes;

namespace DTO.Enums.User;

public enum Gender
{
    [LocalizationKey("enum.gender.male")]
    Male = 1,
    [LocalizationKey("enum.gender.female")]
    Female = 2,
    [LocalizationKey("enum.gender.nonBinary")]
    NonBinary = 3,
    [LocalizationKey("enum.gender.preferNotToSay")]
    PreferNotToSay = 4
}
