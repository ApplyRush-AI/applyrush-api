using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum IssueSeverity
{
    [LocalizationKey("enum.issueSeverity.urgent")]
    Urgent = 1,
    [LocalizationKey("enum.issueSeverity.critical")]
    Critical = 2,
    [LocalizationKey("enum.issueSeverity.optional")]
    Optional = 3
}
