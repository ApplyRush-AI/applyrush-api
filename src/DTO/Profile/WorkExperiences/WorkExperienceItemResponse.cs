using DTO.Profile.WorkExperiences.WorkExperienceBullets;
using DTO.Response;

namespace DTO.Profile.WorkExperiences;

public record WorkExperienceItemResponse
{
    public int Id { get; init; }
    public string JobTitle { get; init; } = null!;
    public string Company { get; init; } = null!;
    public ListItemBaseResponse? JobType { get; init; }
    public string? Location { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public bool IsCurrent { get; init; }
    public string? Summary { get; init; }
    public int OrderIndex { get; init; }
    public IReadOnlyList<WorkExperienceBulletResponse> Bullets { get; init; } = [];
}
