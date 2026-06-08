using DTO.Attributes;

namespace DTO.Enums.Job;

public enum WorkModel
{
    [LocalizationKey("enum.workModel.onsite")]
    Onsite = 1,
    [LocalizationKey("enum.workModel.hybrid")]
    Hybrid = 2,
    [LocalizationKey("enum.workModel.remote")]
    Remote = 3
}
