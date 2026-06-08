using Domain.Entities.Base;
using DTO.Enums.Job;

namespace Domain.Entities.Settings;

public sealed class UserJobPreference : BaseAuditableEntity
{
    private UserJobPreference() { }

    public int UserId { get; private set; }
    public string? DesiredTitle { get; private set; }
    public decimal? SalaryMin { get; private set; }
    public string? Locations { get; private set; }
    public WorkModel? WorkModel { get; private set; }
    public string? IndustryWeights { get; private set; }

    public User.ApplicationUser User { get; } = null!;

    public static UserJobPreference Create(int userId)
    {
        return new UserJobPreference
        {
            UserId = userId
        };
    }

    public void Update(IUserJobPreferenceUpdateData data)
    {
        DesiredTitle = data.DesiredTitle;
        SalaryMin = data.SalaryMin;
        Locations = data.Locations;
        WorkModel = data.WorkModel;
        IndustryWeights = data.IndustryWeights;
    }
}
