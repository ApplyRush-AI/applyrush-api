using Domain.Entities.Subscriptions.UserSubscriptions;

namespace Application.Features.Subscriptions.Data;

public sealed record UserSubscriptionInsertData(int UserId, string StripeCustomerId) : IUserSubscriptionInsertData;
