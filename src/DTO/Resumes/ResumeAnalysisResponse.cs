using DTO.Response;

namespace DTO.Resumes;

public sealed class ResumeAnalysisResponse
{
    public int Id { get; init; }
    public ListItemBaseResponse OverallGrade { get; init; } = null!;
    public int UrgentFixCount { get; init; }
    public int CriticalFixCount { get; init; }
    public int OptionalFixCount { get; init; }
    public IReadOnlyList<ResumeAnalysisIssueResponse> Issues { get; init; } = [];
    public ListItemBaseResponse Status { get; init; } = null!;
    public int CreditsUsed { get; init; }
    public DateTime DateCreated { get; init; }
}
