namespace DTO.Resumes;

public sealed record TailoredResumeExperience
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string Company { get; init; } = null!;
    public string? Location { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
    public bool IsCurrent { get; init; }
    public IReadOnlyList<string> Bullets { get; init; } = [];
}
