using DTO.Attributes;

namespace DTO.Enums.Job;

public enum JobFeedSortBy
{
    [LocalizationKey("enum.jobFeedSortBy.recommended")]
    Recommended = 1,
    [LocalizationKey("enum.jobFeedSortBy.date")]
    Date = 2,
    [LocalizationKey("enum.jobFeedSortBy.match")]
    Match = 3
}
