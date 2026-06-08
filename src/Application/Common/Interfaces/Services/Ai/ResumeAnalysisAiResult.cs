using DTO.Enums.Resume;

namespace Application.Common.Interfaces.Services.Ai;

public sealed record ResumeAnalysisAiResult
{
    public ResumeAnalysisGrade OverallGrade { get; init; }
    public int UrgentFixCount { get; init; }
    public int CriticalFixCount { get; init; }
    public int OptionalFixCount { get; init; }
    public string IssuesJson { get; init; } = null!;
}
