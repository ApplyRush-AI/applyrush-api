using DTO.Enums.Job;

namespace Domain.Entities.Settings;

public interface IUserJobPreferenceUpdateData
{
    string? DesiredTitle { get; }
    decimal? SalaryMin { get; }
    string? Locations { get; }
    WorkModel? WorkModel { get; }
    string? IndustryWeights { get; }
}
