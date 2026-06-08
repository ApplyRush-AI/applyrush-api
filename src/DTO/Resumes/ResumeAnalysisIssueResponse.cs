using DTO.Response;

namespace DTO.Resumes;

public sealed class ResumeAnalysisIssueResponse
{
    public string Id { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public ListItemBaseResponse Severity { get; init; } = null!;
    public string? Section { get; init; }
}
