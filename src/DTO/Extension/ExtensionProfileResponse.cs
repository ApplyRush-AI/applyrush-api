namespace DTO.Extension;

public sealed class ExtensionProfileResponse
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public string? PostalCode { get; init; }
    public string? AddressLine1 { get; init; }
    public string? LinkedInUrl { get; init; }
    public string? GitHubUrl { get; init; }
    public string? WebsiteUrl { get; init; }
    public string? Title { get; init; }
    public IReadOnlyList<string> Skills { get; init; } = [];
    public IReadOnlyList<ExtensionWorkExperienceItemResponse> WorkExperiences { get; init; } = [];
    public IReadOnlyList<ExtensionEducationItemResponse> Educations { get; init; } = [];
}
