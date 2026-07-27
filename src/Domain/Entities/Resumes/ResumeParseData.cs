namespace Domain.Entities.Resumes;

public sealed record ResumeParseData
{
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
    // EEO: only the two fields that genuinely appear on a resume are parsed; the demographic fields
    // (gender, race, veteran, disability, orientation) stay user-entered and are never inferred.
    public string? WorkAuthorization { get; init; }
    public bool? SponsorshipNeeded { get; init; }
    public IReadOnlyList<string> Skills { get; init; } = [];
    public IReadOnlyList<ResumeParseExperienceItem> Experience { get; init; } = [];
    public IReadOnlyList<ResumeParseEducationItem> Education { get; init; } = [];
}
