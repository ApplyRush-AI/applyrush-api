namespace DTO.Admin;

public sealed record AdminDashboardResponse(
    int TotalUsers,
    int ActiveSubscriptions,
    int FreeUsers,
    int ProUsers,
    int PremiumUsers,
    int TotalJobsIndexed,
    DateTime? LastJobSyncAt,
    int TotalApplicationsSubmitted,
    int NewSignupsToday,
    int NewSignupsThisWeek,
    int NewSignupsThisMonth
);
