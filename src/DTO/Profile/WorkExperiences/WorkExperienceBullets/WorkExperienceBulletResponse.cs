namespace DTO.Profile.WorkExperiences.WorkExperienceBullets;

public record WorkExperienceBulletResponse
{
    public int Id { get; init; }
    public string Content { get; init; } = null!;
    public int OrderIndex { get; init; }
}
