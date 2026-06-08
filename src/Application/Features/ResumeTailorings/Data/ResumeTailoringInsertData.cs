using Domain.Entities.Tailoring.ResumeTailorings;

namespace Application.Features.ResumeTailorings.Data;

internal sealed record ResumeTailoringInsertData(
    int UserId,
    int? ResumeId,
    int JobId,
    IReadOnlyList<string> SectionsToEnhance,
    IReadOnlyList<string> KeywordsToInject,
    int CreditsUsed) : IResumeTailoringInsertData;
