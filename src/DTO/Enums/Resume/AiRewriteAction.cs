using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum AiRewriteAction
{
    [LocalizationKey("enum.aiRewriteAction.strongerActionVerbs")]
    StrongerActionVerbs = 1,
    [LocalizationKey("enum.aiRewriteAction.shortenSummary")]
    ShortenSummary = 2,
    [LocalizationKey("enum.aiRewriteAction.removeUnrelatedSkills")]
    RemoveUnrelatedSkills = 3,
    [LocalizationKey("enum.aiRewriteAction.freeForm")]
    FreeForm = 4
}
