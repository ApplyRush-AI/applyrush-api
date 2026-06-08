namespace Application.Common.Interfaces.Services.Ai;

public sealed record ResumeAnalysisFixAiOptions
{
    public int AnalysisId { get; init; }
    public string IssueId { get; init; } = null!;
}
