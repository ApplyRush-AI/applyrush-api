using DTO.Enums.Profile.UserProfile;
using DTO.Profile.Educations;
using DTO.Profile.WorkExperiences;
using DTO.Response;

namespace DTO.Profile;

public record ProfileResponse
{
    public int Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? County { get; init; }
    public string? PostalCode { get; init; }
    public string? AddressLine1 { get; init; }
    public string? LinkedInUrl { get; init; }
    public string? GitHubUrl { get; init; }
    public string? WebsiteUrl { get; init; }
    public string? Title { get; init; }
    public string? Bio { get; init; }
    public int? YearsExperience { get; init; }
    public ListItemBaseResponse LocationMode { get; init; } = null!;
    public string? Location { get; init; }
    public bool OpenToRemote { get; init; }
    public bool H1BSponsorship { get; init; }
    public bool OnboardingCompleted { get; init; }
    public IReadOnlyList<string> Skills { get; init; } = [];
    public IReadOnlyList<WorkExperienceItemResponse> WorkExperiences { get; init; } = [];
    public IReadOnlyList<EducationItemResponse> Educations { get; init; } = [];
    public EeoDataResponse? EeoData { get; init; }
}
