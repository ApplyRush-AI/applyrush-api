using DTO.Enums.Subscription;
using DTO.Enums.User;

namespace DTO.Admin;

public sealed record AdminUserDetailResponse(
    int Id,
    string Email,
    string? FirstName,
    string? LastName,
    SubscriptionPlan Plan,
    SubscriptionStatus SubscriptionStatus,
    UserStatus UserStatus,
    DateTime CreatedAt,
    int ResumeCount,
    int TailoringCount,
    int AnalysisCount,
    int ApplicationCount,
    int TailoringCreditsRemaining,
    int AnalysisCreditsRemaining,
    int AutofillCreditsRemaining
);
