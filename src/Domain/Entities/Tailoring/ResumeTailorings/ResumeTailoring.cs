using Domain.Entities.Base;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.Resumes.Resume;
using Domain.Entities.User;
using Domain.Events.Tailoring;
using DTO.Enums.Resume;
using System.Text.Json;

namespace Domain.Entities.Tailoring.ResumeTailorings;

public sealed class ResumeTailoring : BaseAuditableEntity
{
    private ResumeTailoring() { }

    public int UserId { get; private set; }
    public int? ResumeId { get; private set; }
    public int JobId { get; private set; }
    public string TailoredContent { get; private set; } = null!;
    public decimal ScoreBefore { get; private set; }
    public decimal ScoreAfter { get; private set; }
    public int CreditsUsed { get; private set; }
    public TailoringStatus Status { get; private set; }
    public string SectionsToEnhanceJson { get; private set; } = "[]";
    public string KeywordsToInjectJson { get; private set; } = "[]";
    public string ChangesJson { get; private set; } = "[]";

    public IReadOnlyList<string> SectionsToEnhance =>
        JsonSerializer.Deserialize<List<string>>(SectionsToEnhanceJson) ?? [];

    public IReadOnlyList<string> KeywordsToInject =>
        JsonSerializer.Deserialize<List<string>>(KeywordsToInjectJson) ?? [];

    public IReadOnlyList<string> Changes =>
        JsonSerializer.Deserialize<List<string>>(ChangesJson) ?? [];

    public ApplicationUser User { get; } = null!;
    public Resume? Resume { get; } = null!;
    public JobListing Job { get; } = null!;

    public static ResumeTailoring Create(IResumeTailoringInsertData data)
    {
        var tailoring = new ResumeTailoring
        {
            UserId = data.UserId,
            ResumeId = data.ResumeId,
            JobId = data.JobId,
            TailoredContent = JsonSerializer.Serialize(new { }),
            ScoreBefore = 0,
            ScoreAfter = 0,
            CreditsUsed = data.CreditsUsed,
            Status = TailoringStatus.Processing,
            SectionsToEnhanceJson = JsonSerializer.Serialize(data.SectionsToEnhance),
            KeywordsToInjectJson = JsonSerializer.Serialize(data.KeywordsToInject)
        };

        tailoring.AddDomainEvent(new ResumeTailoringCreatedEvent(tailoring));
        return tailoring;
    }

    public void Complete(IResumeTailoringCompleteData data)
    {
        TailoredContent = data.TailoredContent;
        ScoreBefore = data.ScoreBefore;
        ScoreAfter = data.ScoreAfter;
        ChangesJson = JsonSerializer.Serialize(data.Changes);
        Status = data.Status;
        AddDomainEvent(new ResumeTailoringCompletedEvent(this));
    }

    public void UpdateContent(string tailoredContent)
    {
        TailoredContent = tailoredContent;
    }

    public void MarkFailed()
    {
        Status = TailoringStatus.Failed;
    }
}
