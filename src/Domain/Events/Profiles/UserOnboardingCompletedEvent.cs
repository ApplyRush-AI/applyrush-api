using Domain.Entities.Profiles;
using Domain.Entities.Profiles.UserProfiles;

namespace Domain.Events.Profiles;

public sealed class UserOnboardingCompletedEvent : BaseEvent
{
    public UserOnboardingCompletedEvent(UserProfile profile)
    {
        Profile = profile;
    }

    public UserProfile Profile { get; }
}
