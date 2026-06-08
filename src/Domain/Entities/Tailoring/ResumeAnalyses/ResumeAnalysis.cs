using Domain.Entities.Base;
using Domain.Entities.User;
using Domain.Events.Tailoring;
using DTO.Enums.Resume;

namespace Domain.Entities.Tailoring.ResumeAnalyses;

public sealed class ResumeAnalysis : BaseAuditableEntity
{
    private ResumeAnalysis() { }

    public int UserId { get; private set; }
    public ResumeAnalysisGrade OverallGrade { get; private set; }
    public int UrgentFixCount { get; private set; }
    public int CriticalFixCount { get; private set; }
    public int OptionalFixCount { get; private set; }
    public string Issues { get; private set; } = "[]";
    public ResumeAnalysisStatus Status { get; private set; }
    public int CreditsUsed { get; private set; }

    public ApplicationUser User { get; } = null!;

    public static ResumeAnalysis Create(IResumeAnalysisInsertData data)
    {
        var analysis = new ResumeAnalysis
        {
            UserId = data.UserId,
            CreditsUsed = data.CreditsUsed,
            OverallGrade = ResumeAnalysisGrade.A,
            Status = ResumeAnalysisStatus.Analyzing,
            Issues = "[]"
        };

        analysis.AddDomainEvent(new ResumeAnalysisCreatedEvent(analysis));
        return analysis;
    }

    public void Complete(IResumeAnalysisCompleteData data)
    {
        OverallGrade = data.OverallGrade;
        UrgentFixCount = data.UrgentFixCount;
        CriticalFixCount = data.CriticalFixCount;
        OptionalFixCount = data.OptionalFixCount;
        Issues = data.Issues;
        Status = data.Status;
        AddDomainEvent(new ResumeAnalysisCompletedEvent(this));
    }

    public void MarkFailed()
    {
        Status = ResumeAnalysisStatus.Failed;
    }
}
