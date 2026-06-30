using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum ResumeTemplate
{
    [LocalizationKey("enum.resumeTemplate.standard")]
    Standard = 1,
    [LocalizationKey("enum.resumeTemplate.compact")]
    Compact = 2,
    [LocalizationKey("enum.resumeTemplate.centered")]
    Centered = 3
}
