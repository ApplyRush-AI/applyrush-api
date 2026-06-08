namespace Domain.Entities.Tailoring.ResumeTailorings;

public interface IResumeTailoringInsertData
{
    int UserId { get; }
    int? ResumeId { get; }
    int JobId { get; }
    IReadOnlyList<string> SectionsToEnhance { get; }
    IReadOnlyList<string> KeywordsToInject { get; }
    int CreditsUsed { get; }
}
