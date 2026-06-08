namespace Application.Common.Interfaces.Services.Ai;

public sealed record ResumeAnalysisFixAiResult
{
    public string Section { get; init; } = null!;
    public string UpdatedContent { get; init; } = null!;
}
