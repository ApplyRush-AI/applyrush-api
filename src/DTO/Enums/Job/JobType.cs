using DTO.Attributes;

namespace DTO.Enums.Job;

public enum JobType
{
    [LocalizationKey("enum.jobType.fullTime")]
    FullTime = 1,
    [LocalizationKey("enum.jobType.partTime")]
    PartTime = 2,
    [LocalizationKey("enum.jobType.contract")]
    Contract = 3,
    [LocalizationKey("enum.jobType.internship")]
    Internship = 4
}
