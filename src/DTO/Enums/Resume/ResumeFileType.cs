using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum ResumeFileType
{
    [LocalizationKey("enum.resumeFileType.pdf")]
    Pdf = 1,
    [LocalizationKey("enum.resumeFileType.word")]
    Word = 2
}
