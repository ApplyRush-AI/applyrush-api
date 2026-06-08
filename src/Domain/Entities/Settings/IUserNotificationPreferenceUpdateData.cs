namespace Domain.Entities.Settings;

public interface IUserNotificationPreferenceUpdateData
{
    bool? NewJobMatches { get; }
    bool? ApplicationUpdates { get; }
    bool? WeeklyDigest { get; }
    bool? ProfileViews { get; }
    bool? MarketingEmails { get; }
}
