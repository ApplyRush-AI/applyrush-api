using DTO.Attributes;

namespace DTO.Enums.Profile.EeoData;

public enum SexualOrientation
{
    [LocalizationKey("enum.sexualOrientation.asexual")]
    Asexual = 1,
    [LocalizationKey("enum.sexualOrientation.bisexual")]
    Bisexual = 2,
    [LocalizationKey("enum.sexualOrientation.gay")]
    Gay = 3,
    [LocalizationKey("enum.sexualOrientation.heterosexual")]
    Heterosexual = 4,
    [LocalizationKey("enum.sexualOrientation.lesbian")]
    Lesbian = 5,
    [LocalizationKey("enum.sexualOrientation.pansexual")]
    Pansexual = 6,
    [LocalizationKey("enum.sexualOrientation.queer")]
    Queer = 7,
    [LocalizationKey("enum.sexualOrientation.preferNotToSay")]
    PreferNotToSay = 8
}
