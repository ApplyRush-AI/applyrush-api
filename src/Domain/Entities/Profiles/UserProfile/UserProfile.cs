using Domain.Entities.Base;
using Domain.Entities.Profiles.Educations;
using Domain.Entities.Profiles.EeoDatas;
using Domain.Entities.Profiles.UserJobTypeReferences;
using Domain.Entities.Profiles.UserPreferredJobFunctions;
using Domain.Entities.Profiles.UserSkills;
using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Entities.User;
using Domain.Events.Profiles;
using DTO.Enums.Profile.UserProfile;

namespace Domain.Entities.Profiles.UserProfiles;

public sealed class UserProfile : BaseAuditableEntity
{
    private UserProfile() { }

    public int UserId { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Country { get; private set; }
    public string? City { get; private set; }
    public string? County { get; private set; }
    public string? PostalCode { get; private set; }
    public string? AddressLine1 { get; private set; }
    public string? LinkedInUrl { get; private set; }
    public string? GitHubUrl { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string? Title { get; private set; }
    public string? Bio { get; private set; }
    public int? YearsExperience { get; private set; }
    public LocationMode LocationMode { get; private set; }
    public string? Location { get; private set; }
    public bool OpenToRemote { get; private set; }
    public bool H1BSponsorship { get; private set; }
    public bool OnboardingCompleted { get; private set; }

    public ApplicationUser User { get; } = null!;
    public ICollection<WorkExperience> WorkExperiences { get; } = new List<WorkExperience>();
    public ICollection<Education> Educations { get; } = new List<Education>();
    public ICollection<UserSkill> Skills { get; } = new List<UserSkill>();
    public ICollection<UserJobTypePreference> JobTypePreferences { get; } = new List<UserJobTypePreference>();
    public ICollection<UserPreferredJobFunction> PreferredJobFunctions { get; } = new List<UserPreferredJobFunction>();
    public EeoData? EeoData { get; private set; }

    public static UserProfile Create(int userId)
    {
        return new UserProfile
        {
            UserId = userId
        };
    }

    public void Update(IUserProfileUpdateData data)
    {
        FirstName = data.FirstName;
        LastName = data.LastName;
        Email = data.Email;
        Phone = data.Phone;
        Country = data.Country;
        City = data.City;
        County = data.County;
        PostalCode = data.PostalCode;
        AddressLine1 = data.AddressLine1;
        LinkedInUrl = data.LinkedInUrl;
        GitHubUrl = data.GitHubUrl;
        WebsiteUrl = data.WebsiteUrl;
        Title = data.Title;
        Bio = data.Bio;
        LocationMode = data.LocationMode;
        Location = data.Location;
        OpenToRemote = data.OpenToRemote;
        H1BSponsorship = data.H1BSponsorship;

        AddDomainEvent(new UserProfileUpdatedEvent(this));
    }

    public void UpdateOnboarding(IUserOnboardingUpdateData data)
    {
        LocationMode = data.LocationMode;
        Location = data.Location;
        OpenToRemote = data.OpenToRemote;
        H1BSponsorship = data.H1BSponsorship;
        OnboardingCompleted = true;

        AddDomainEvent(new UserOnboardingCompletedEvent(this));
    }

    public void UpdateJobPreferences(IUserJobPreferencesUpdateData data)
    {
        LocationMode = data.LocationMode;
        Location = data.Location;
        OpenToRemote = data.OpenToRemote;
        H1BSponsorship = data.H1BSponsorship;
    }

    public void SetYearsExperience(int? years)
    {
        YearsExperience = years;
    }

    public void SetFirstName(string? value) => FirstName = value;
    public void SetLastName(string? value) => LastName = value;
    public void SetEmail(string? value) => Email = value;
    public void SetPhone(string? value) => Phone = value;
    public void SetLinkedInUrl(string? value) => LinkedInUrl = value;
    public void SetGitHubUrl(string? value) => GitHubUrl = value;
    public void SetWebsiteUrl(string? value) => WebsiteUrl = value;
    public void SetTitle(string? value) => Title = value;
    public void SetBio(string? value) => Bio = value;
    public void SetCountry(string? value) => Country = value;
    public void SetCity(string? value) => City = value;
}
