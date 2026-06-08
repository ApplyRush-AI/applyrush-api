namespace DTO.Profile.Settings.NotificationPreferences;

public record NotificationPreferenceResponse
{
    public bool NewJobMatches { get; init; }
    public bool ApplicationUpdates { get; init; }
    public bool WeeklyDigest { get; init; }
    public bool ProfileViews { get; init; }
    public bool MarketingEmails { get; init; }
}
