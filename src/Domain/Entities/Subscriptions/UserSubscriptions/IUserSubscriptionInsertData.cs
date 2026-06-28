namespace Domain.Entities.Subscriptions.UserSubscriptions;

public interface IUserSubscriptionInsertData
{
    int UserId { get; }
    string StripeCustomerId { get; }
}
