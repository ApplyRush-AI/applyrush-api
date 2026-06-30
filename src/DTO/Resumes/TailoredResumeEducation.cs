namespace DTO.Resumes;

public sealed record TailoredResumeEducation
{
    public int Id { get; init; }
    public string School { get; init; } = null!;
    public string Degree { get; init; } = null!;
    public string? Major { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
}
