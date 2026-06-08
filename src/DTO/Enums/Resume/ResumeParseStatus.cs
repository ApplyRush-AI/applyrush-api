using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum ResumeParseStatus
{
    [LocalizationKey("enum.resumeParseStatus.pending")]
    Pending = 1,
    [LocalizationKey("enum.resumeParseStatus.processing")]
    Processing = 2,
    [LocalizationKey("enum.resumeParseStatus.completed")]
    Completed = 3,
    [LocalizationKey("enum.resumeParseStatus.failed")]
    Failed = 4
}
