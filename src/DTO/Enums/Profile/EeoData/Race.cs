using DTO.Attributes;

namespace DTO.Enums.Profile.EeoData;

public enum Race
{
    [LocalizationKey("enum.race.americanIndianOrAlaskanNative")]
    AmericanIndianOrAlaskanNative = 1,
    [LocalizationKey("enum.race.asian")]
    Asian = 2,
    [LocalizationKey("enum.race.blackOrAfricanAmerican")]
    BlackOrAfricanAmerican = 3,
    [LocalizationKey("enum.race.hispanicOrLatino")]
    HispanicOrLatino = 4,
    [LocalizationKey("enum.race.white")]
    White = 5,
    [LocalizationKey("enum.race.nativeHawaiianOrPacificIslander")]
    NativeHawaiianOrPacificIslander = 6,
    [LocalizationKey("enum.race.twoOrMoreRaces")]
    TwoOrMoreRaces = 7,
    [LocalizationKey("enum.race.preferNotToSay")]
    PreferNotToSay = 8
}
