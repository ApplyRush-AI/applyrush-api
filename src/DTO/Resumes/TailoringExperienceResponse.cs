namespace DTO.Resumes;

public sealed class TailoringExperienceResponse
{
    public int Id { get; init; }
    public IReadOnlyList<string> Bullets { get; init; } = [];
}
