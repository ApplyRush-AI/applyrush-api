using DTO.Enums.Profile.UserProfile;

namespace Domain.Entities.Profiles.UserProfiles;

public interface IUserProfileBaseData
{
    string? FirstName { get; }
    string? LastName { get; }
    string? Email { get; }
    string? Phone { get; }
    string? Country { get; }
    string? City { get; }
    string? County { get; }
    string? PostalCode { get; }
    string? AddressLine1 { get; }
    string? LinkedInUrl { get; }
    string? GitHubUrl { get; }
    string? WebsiteUrl { get; }
    string? Title { get; }
    string? Bio { get; }
    LocationMode LocationMode { get; }
    string? Location { get; }
    bool OpenToRemote { get; }
    bool H1BSponsorship { get; }
}
