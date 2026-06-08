using DTO.Attributes;

namespace DTO.Enums.Job;

public enum JobSource
{
    [LocalizationKey("enum.jobSource.jSearch")]
    JSearch = 1,
    [LocalizationKey("enum.jobSource.adzuna")]
    Adzuna = 2
}
