using Domain.Entities.Base;
using Domain.Entities.Profiles.UserProfiles;

namespace Domain.Entities.Profiles.UserPreferredJobFunctions;

public sealed class UserPreferredJobFunction : BaseEntity
{
    private UserPreferredJobFunction() { }

    public int UserProfileId { get; private set; }
    public int JobFunctionId { get; private set; }

    public UserProfile UserProfile { get; } = null!;
    public JobFunctions.JobFunction JobFunction { get; } = null!;

    public static UserPreferredJobFunction Create(int userProfileId, int jobFunctionId)
    {
        return new UserPreferredJobFunction
        {
            UserProfileId = userProfileId,
            JobFunctionId = jobFunctionId
        };
    }
}
