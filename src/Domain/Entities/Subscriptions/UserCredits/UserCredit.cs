using Domain.Entities.Base;
using Domain.Entities.User;
using Domain.Events.Subscriptions;

namespace Domain.Entities.Subscriptions.UserCredits;

public sealed class UserCredit : BaseAuditableEntity
{
    private UserCredit() { }

    public int UserId { get; private set; }
    public int TailoringCreditsTotal { get; private set; }
    public int TailoringCreditsUsed { get; private set; }
    public int AnalysisCreditsTotal { get; private set; }
    public int AnalysisCreditsUsed { get; private set; }
    public int AutofillCreditsTotal { get; private set; }
    public int AutofillCreditsUsed { get; private set; }
    public DateTime LastResetAt { get; private set; }

    public ApplicationUser User { get; } = null!;

    public static UserCredit Create(IUserCreditInsertData data)
    {
        return new UserCredit
        {
            UserId = data.UserId,
            TailoringCreditsTotal = data.TailoringCreditsTotal,
            TailoringCreditsUsed = 0,
            AnalysisCreditsTotal = data.AnalysisCreditsTotal,
            AnalysisCreditsUsed = 0,
            AutofillCreditsTotal = data.AutofillCreditsTotal,
            AutofillCreditsUsed = 0,
            LastResetAt = data.LastResetAt
        };
    }

    public void DeductTailoring()
    {
        TailoringCreditsUsed++;
        AddDomainEvent(new UserCreditDeductedEvent(this, CreditTypes.Tailoring));
    }

    public void DeductAnalysis()
    {
        AnalysisCreditsUsed++;
        AddDomainEvent(new UserCreditDeductedEvent(this, CreditTypes.Analysis));
    }

    public void DeductAutofill()
    {
        AutofillCreditsUsed++;
        AddDomainEvent(new UserCreditDeductedEvent(this, CreditTypes.Autofill));
    }

    public void Reset(int tailoringTotal, int analysisTotal, int autofillTotal, DateTime resetAt)
    {
        TailoringCreditsTotal = tailoringTotal;
        TailoringCreditsUsed = 0;
        AnalysisCreditsTotal = analysisTotal;
        AnalysisCreditsUsed = 0;
        AutofillCreditsTotal = autofillTotal;
        AutofillCreditsUsed = 0;
        LastResetAt = resetAt;
        AddDomainEvent(new UserCreditResetEvent(this));
    }
}
