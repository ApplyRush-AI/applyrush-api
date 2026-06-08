using DTO.Attributes;

namespace DTO.Enums.JobOffer;

public enum JobOfferFeedSortField
{
    [LocalizationKey("enum.jobOfferFeedSortField.recommended")]
    Recommended = 1,
    [LocalizationKey("enum.jobOfferFeedSortField.date")]
    Date = 2,
    [LocalizationKey("enum.jobOfferFeedSortField.match")]
    Match = 3
}
