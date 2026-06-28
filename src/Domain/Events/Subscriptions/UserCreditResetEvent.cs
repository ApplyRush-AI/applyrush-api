using Domain.Entities.Subscriptions.UserCredits;

namespace Domain.Events.Subscriptions;

public sealed class UserCreditResetEvent : BaseEvent
{
    public UserCreditResetEvent(UserCredit credit)
    {
        Credit = credit;
    }

    public UserCredit Credit { get; }
}
