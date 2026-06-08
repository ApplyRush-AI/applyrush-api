using DTO.Response;

namespace DTO.Profile.Settings.JobPreferences;

public record JobPreferenceResponse
{
    public string? DesiredTitle { get; init; }
    public decimal? SalaryMin { get; init; }
    public string? Locations { get; init; }
    public ListItemBaseResponse? WorkModel { get; init; }
    public string? IndustryWeights { get; init; }
}
