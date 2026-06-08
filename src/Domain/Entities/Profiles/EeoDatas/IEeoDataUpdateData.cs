using DTO.Enums.Profile.EeoData;
using DTO.Enums.User;

namespace Domain.Entities.Profiles.EeoDatas;

public interface IEeoDataUpdateData
{
    WorkAuthorization? WorkAuthorization { get; }
    bool? SponsorshipNeeded { get; }
    DisabilityStatus? Disability { get; }
    VeteranStatus? VeteranStatus { get; }
    Gender? Gender { get; }
    bool? IsLgbtq { get; }
    Race? Race { get; }
    bool? IsHispanicLatino { get; }
    SexualOrientation? SexualOrientation { get; }
}
