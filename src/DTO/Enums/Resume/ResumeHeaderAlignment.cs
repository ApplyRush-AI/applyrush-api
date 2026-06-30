using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum ResumeHeaderAlignment
{
    [LocalizationKey("enum.resumeHeaderAlignment.left")]
    Left = 1,
    [LocalizationKey("enum.resumeHeaderAlignment.center")]
    Center = 2,
    [LocalizationKey("enum.resumeHeaderAlignment.right")]
    Right = 3
}
