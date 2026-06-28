using Domain.Entities.Subscriptions.UserCredits;

namespace Domain.Events.Subscriptions;

public sealed class UserCreditDeductedEvent : BaseEvent
{
    public UserCreditDeductedEvent(UserCredit credit, string creditType)
    {
        Credit = credit;
        CreditType = creditType;
    }

    public UserCredit Credit { get; }
    public string CreditType { get; }
}
