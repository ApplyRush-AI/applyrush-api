using DTO.Attributes;

namespace DTO.Enums.JobApplication;

public enum ApplicationStatus
{
    [LocalizationKey("enum.applicationStatus.applied")]
    Applied = 1,
    [LocalizationKey("enum.applicationStatus.interview")]
    Interview = 2,
    [LocalizationKey("enum.applicationStatus.offer")]
    Offer = 3,
    [LocalizationKey("enum.applicationStatus.rejected")]
    Rejected = 4,
    [LocalizationKey("enum.applicationStatus.withdrawn")]
    Withdrawn = 5,
    [LocalizationKey("enum.applicationStatus.deleted")]
    Deleted = 6
}
