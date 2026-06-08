namespace Domain.Entities.Resumes;

public sealed record ResumeParseExperienceItem
{
    public string? JobTitle { get; init; }
    public string? Company { get; init; }
    public string? Location { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
    public bool IsCurrent { get; init; }
    public string? Summary { get; init; }
    public IReadOnlyList<string> Bullets { get; init; } = [];
}
