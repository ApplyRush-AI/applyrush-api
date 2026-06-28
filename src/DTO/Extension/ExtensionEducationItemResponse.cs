namespace DTO.Extension;

public sealed class ExtensionEducationItemResponse
{
    public string School { get; init; } = null!;
    public string? Major { get; init; }
    public string DegreeType { get; init; } = null!;
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
}
