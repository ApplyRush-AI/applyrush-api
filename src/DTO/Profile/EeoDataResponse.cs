using DTO.Response;

namespace DTO.Profile;

public record EeoDataResponse
{
    public ListItemBaseResponse? WorkAuthorization { get; init; }
    public bool? SponsorshipNeeded { get; init; }
    public ListItemBaseResponse? Disability { get; init; }
    public ListItemBaseResponse? VeteranStatus { get; init; }
    public ListItemBaseResponse? Gender { get; init; }
    public bool? IsLgbtq { get; init; }
    public ListItemBaseResponse? Race { get; init; }
    public bool? IsHispanicLatino { get; init; }
    public ListItemBaseResponse? SexualOrientation { get; init; }
}
