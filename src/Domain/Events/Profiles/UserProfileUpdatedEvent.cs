using Domain.Entities.Profiles;
using Domain.Entities.Profiles.UserProfiles;

namespace Domain.Events.Profiles;

public sealed class UserProfileUpdatedEvent : BaseEvent
{
    public UserProfileUpdatedEvent(UserProfile profile)
    {
        Profile = profile;
    }

    public UserProfile Profile { get; }
}
