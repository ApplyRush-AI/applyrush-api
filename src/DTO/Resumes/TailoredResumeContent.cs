namespace DTO.Resumes;

public sealed record TailoredResumeContent
{
    public string? Summary { get; init; }
    public IReadOnlyList<string> Skills { get; init; } = [];
    public IReadOnlyList<TailoredResumeExperience> Experience { get; init; } = [];
    public IReadOnlyList<TailoredResumeEducation> Education { get; init; } = [];
}
