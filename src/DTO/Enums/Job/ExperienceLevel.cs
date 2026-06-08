using DTO.Attributes;

namespace DTO.Enums.Job;

public enum ExperienceLevel
{
    [LocalizationKey("enum.experienceLevel.entryLevel")]
    EntryLevel = 1,
    [LocalizationKey("enum.experienceLevel.midLevel")]
    MidLevel = 2,
    [LocalizationKey("enum.experienceLevel.senior")]
    Senior = 3,
    [LocalizationKey("enum.experienceLevel.lead")]
    Lead = 4,
    [LocalizationKey("enum.experienceLevel.executive")]
    Executive = 5
}
