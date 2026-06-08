namespace DTO.Resumes;

public sealed record ResumeParsedEducationItem
{
    public string? School { get; init; }
    public string? Major { get; init; }
    public string? Degree { get; init; }
    public decimal? Gpa { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
    public bool IsCurrent { get; init; }
}
