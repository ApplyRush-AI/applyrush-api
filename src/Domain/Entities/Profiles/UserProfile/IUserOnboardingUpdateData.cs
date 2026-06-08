using DTO.Enums.Job;
using DTO.Enums.Profile.UserProfile;

namespace Domain.Entities.Profiles.UserProfiles;

public interface IUserOnboardingUpdateData
{
    IReadOnlyList<int> JobFunctionIds { get; }
    IReadOnlyList<JobType> JobTypes { get; }
    LocationMode LocationMode { get; }
    string? Location { get; }
    bool OpenToRemote { get; }
    bool H1BSponsorship { get; }
}
