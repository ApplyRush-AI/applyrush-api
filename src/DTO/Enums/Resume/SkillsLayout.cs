using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum SkillsLayout
{
    [LocalizationKey("enum.skillsLayout.list")]
    List = 1,
    [LocalizationKey("enum.skillsLayout.bulleted")]
    Bulleted = 2,
    [LocalizationKey("enum.skillsLayout.twoColumn")]
    TwoColumn = 3
}
