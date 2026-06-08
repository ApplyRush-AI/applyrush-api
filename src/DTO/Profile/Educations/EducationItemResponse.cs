using DTO.Response;

namespace DTO.Profile.Educations;

public record EducationItemResponse
{
    public int Id { get; init; }
    public string School { get; init; } = null!;
    public string? Major { get; init; }
    public ListItemBaseResponse DegreeType { get; init; } = null!;
    public decimal? Gpa { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public bool IsCurrent { get; init; }
    public int OrderIndex { get; init; }
}
