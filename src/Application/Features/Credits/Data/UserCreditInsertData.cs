using Domain.Entities.Subscriptions.UserCredits;

namespace Application.Features.Credits.Data;

public sealed record UserCreditInsertData(
    int UserId,
    int TailoringCreditsTotal,
    int AnalysisCreditsTotal,
    int AutofillCreditsTotal,
    DateTime LastResetAt) : IUserCreditInsertData;
