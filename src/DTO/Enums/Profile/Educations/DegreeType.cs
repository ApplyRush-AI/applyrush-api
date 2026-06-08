using DTO.Attributes;

namespace DTO.Enums.Profile.Education;

public enum DegreeType
{
    [LocalizationKey("enum.degreeType.associate")]
    Associate = 1,
    [LocalizationKey("enum.degreeType.bachelor")]
    Bachelor = 2,
    [LocalizationKey("enum.degreeType.master")]
    Master = 3,
    [LocalizationKey("enum.degreeType.doctorate")]
    Doctorate = 4,
    [LocalizationKey("enum.degreeType.certificate")]
    Certificate = 5,
    [LocalizationKey("enum.degreeType.other")]
    Other = 6
}
