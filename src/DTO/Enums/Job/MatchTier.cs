using DTO.Attributes;

namespace DTO.Enums.Job;

public enum MatchTier
{
    [LocalizationKey("enum.matchTier.strongMatch")]
    StrongMatch = 1,
    [LocalizationKey("enum.matchTier.goodMatch")]
    GoodMatch = 2,
    [LocalizationKey("enum.matchTier.fairMatch")]
    FairMatch = 3
}
