using DTO.Attributes;

namespace DTO.Enums.Extension;

public enum ExtensionSessionStatus
{
    [LocalizationKey("enum.extensionSessionStatus.active")]
    Active = 1,
    [LocalizationKey("enum.extensionSessionStatus.completed")]
    Completed = 2,
    [LocalizationKey("enum.extensionSessionStatus.failed")]
    Failed = 3
}
