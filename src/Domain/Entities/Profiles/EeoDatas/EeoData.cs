using Domain.Entities.Base;
using Domain.Entities.Profiles.UserProfiles;
using DTO.Enums.Profile.EeoData;
using DTO.Enums.User;

namespace Domain.Entities.Profiles.EeoDatas;

public sealed class EeoData : BaseAuditableEntity
{
    private EeoData() { }

    public int UserProfileId { get; private set; }
    public WorkAuthorization? WorkAuthorization { get; private set; }
    public bool? SponsorshipNeeded { get; private set; }
    public DisabilityStatus? Disability { get; private set; }
    public VeteranStatus? VeteranStatus { get; private set; }
    public Gender? Gender { get; private set; }
    public bool? IsLgbtq { get; private set; }
    public Race? Race { get; private set; }
    public bool? IsHispanicLatino { get; private set; }
    public SexualOrientation? SexualOrientation { get; private set; }

    public UserProfile UserProfile { get; } = null!;

    public static EeoData Create(int userProfileId)
    {
        return new EeoData
        {
            UserProfileId = userProfileId
        };
    }

    public void Update(IEeoDataUpdateData data)
    {
        WorkAuthorization = data.WorkAuthorization;
        SponsorshipNeeded = data.SponsorshipNeeded;
        Disability = data.Disability;
        VeteranStatus = data.VeteranStatus;
        Gender = data.Gender;
        IsLgbtq = data.IsLgbtq;
        Race = data.Race;
        IsHispanicLatino = data.IsHispanicLatino;
        SexualOrientation = data.SexualOrientation;
    }

    public void SetWorkAuthorization(WorkAuthorization? value)
    {
        WorkAuthorization = value;
    }
}
