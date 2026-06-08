using Domain.Entities.Base;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums.Job;

namespace Domain.Entities.Profiles.UserJobTypeReferences;

public sealed class UserJobTypePreference : BaseEntity
{
    private UserJobTypePreference() { }

    public int UserProfileId { get; private set; }
    public JobType JobType { get; private set; }

    public UserProfile UserProfile { get; } = null!;

    public static UserJobTypePreference Create(int userProfileId, JobType jobType)
    {
        return new UserJobTypePreference
        {
            UserProfileId = userProfileId,
            JobType = jobType
        };
    }
}
