namespace DTO.Extension;

public sealed class ExtensionWorkExperienceItemResponse
{
    public string JobTitle { get; init; } = null!;
    public string Company { get; init; } = null!;
    public string? Location { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public bool IsCurrent { get; init; }
    public string? Summary { get; init; }
    public IReadOnlyList<string> Bullets { get; init; } = [];
}
