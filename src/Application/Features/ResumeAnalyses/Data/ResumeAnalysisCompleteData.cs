using Domain.Entities.Tailoring.ResumeAnalyses;
using DTO.Enums.Resume;

namespace Application.Features.ResumeAnalyses.Data;

internal sealed record ResumeAnalysisCompleteData : IResumeAnalysisCompleteData
{
    public ResumeAnalysisGrade OverallGrade { get; init; }
    public int UrgentFixCount { get; init; }
    public int CriticalFixCount { get; init; }
    public int OptionalFixCount { get; init; }
    public string Issues { get; init; } = null!;
    public ResumeAnalysisStatus Status { get; init; }
}
