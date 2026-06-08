using Domain.Entities.Base;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums.Job;

namespace Domain.Entities.Profiles.UserSkills;

public sealed class UserSkill : BaseEntity
{
    private UserSkill() { }

    public int UserProfileId { get; private set; }
    public string Name { get; private set; } = null!;
    public int OrderIndex { get; private set; }

    public UserProfile UserProfile { get; } = null!;

    public static UserSkill Create(int userProfileId, string name, int orderIndex)
    {
        return new UserSkill
        {
            UserProfileId = userProfileId,
            Name = name,
            OrderIndex = orderIndex
        };
    }
}
