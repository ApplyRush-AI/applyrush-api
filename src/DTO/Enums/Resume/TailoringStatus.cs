using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum TailoringStatus
{
    [LocalizationKey("enum.tailoringStatus.processing")]
    Processing = 1,
    [LocalizationKey("enum.tailoringStatus.completed")]
    Completed = 2,
    [LocalizationKey("enum.tailoringStatus.failed")]
    Failed = 3
}
