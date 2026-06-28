using Application.Common.Interfaces.Services;
using Domain.Entities.Subscriptions.UserSubscriptions;
using DTO.Enums.Subscription;

namespace Application.Features.Subscriptions.Commands;

internal sealed record SubscriptionUpdateData(StripeWebhookEvent Evt) : IUserSubscriptionUpdateData
{
    public SubscriptionStatus? StatusOverride { get; init; }

    public string? StripeSubscriptionId => Evt.StripeSubscriptionId;
    public SubscriptionPlan Plan => Evt.Plan ?? SubscriptionPlan.Free;
    public BillingInterval? BillingInterval => Evt.Interval;
    public SubscriptionStatus Status => StatusOverride ?? Evt.Status ?? SubscriptionStatus.Active;
    public DateTime? CurrentPeriodStart => Evt.CurrentPeriodStart;
    public DateTime? CurrentPeriodEnd => Evt.CurrentPeriodEnd;
    public DateTime? CanceledAt => Evt.CanceledAt;
}
