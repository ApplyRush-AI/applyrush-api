namespace Domain.Entities.Subscriptions.UserCredits;

public interface IUserCreditInsertData
{
    int UserId { get; }
    int TailoringCreditsTotal { get; }
    int AnalysisCreditsTotal { get; }
    int AutofillCreditsTotal { get; }
    DateTime LastResetAt { get; }
}
