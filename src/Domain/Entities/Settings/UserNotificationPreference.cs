using Domain.Entities.Base;

namespace Domain.Entities.Settings;

public sealed class UserNotificationPreference : BaseAuditableEntity
{
    private UserNotificationPreference() { }

    public int UserId { get; private set; }
    public bool NewJobMatches { get; private set; }
    public bool ApplicationUpdates { get; private set; }
    public bool WeeklyDigest { get; private set; }
    public bool ProfileViews { get; private set; }
    public bool MarketingEmails { get; private set; }

    public User.ApplicationUser User { get; } = null!;

    public static UserNotificationPreference Create(int userId)
    {
        return new UserNotificationPreference
        {
            UserId = userId,
            NewJobMatches = true,
            ApplicationUpdates = true,
            WeeklyDigest = true,
            ProfileViews = false,
            MarketingEmails = false
        };
    }

    public void Update(IUserNotificationPreferenceUpdateData data)
    {
        if (data.NewJobMatches.HasValue) NewJobMatches = data.NewJobMatches.Value;
        if (data.ApplicationUpdates.HasValue) ApplicationUpdates = data.ApplicationUpdates.Value;
        if (data.WeeklyDigest.HasValue) WeeklyDigest = data.WeeklyDigest.Value;
        if (data.ProfileViews.HasValue) ProfileViews = data.ProfileViews.Value;
        if (data.MarketingEmails.HasValue) MarketingEmails = data.MarketingEmails.Value;
    }
}
