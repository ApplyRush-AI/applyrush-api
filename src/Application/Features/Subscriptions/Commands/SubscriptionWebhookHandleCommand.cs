using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Domain.Entities.Subscriptions.UserSubscriptions;
using Domain.Interfaces;
using DTO.Enums.Subscription;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Subscriptions.Commands;

public sealed record SubscriptionWebhookHandleCommand(
    string StripeSignature,
    string RawPayload
) : ICommand;

public sealed class SubscriptionWebhookHandleCommandHandler : ICommandHandler<SubscriptionWebhookHandleCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IStripeService _stripeService;
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionWebhookHandleCommandHandler(
        IApplicationDbContext dbContext,
        IStripeService stripeService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _stripeService = stripeService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SubscriptionWebhookHandleCommand command, CancellationToken cancellationToken)
    {
        var webhookEvent = await _stripeService.ParseWebhookEventAsync(
            command.RawPayload, command.StripeSignature, cancellationToken);

        switch (webhookEvent.EventType)
        {
            case "checkout.session.completed":
            case "customer.subscription.updated":
                await HandleSubscriptionUpdatedAsync(webhookEvent, cancellationToken);
                break;

            case "customer.subscription.deleted":
                await HandleSubscriptionDeletedAsync(webhookEvent, cancellationToken);
                break;

            case "invoice.payment_failed":
                await HandlePaymentFailedAsync(webhookEvent, cancellationToken);
                break;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleSubscriptionUpdatedAsync(StripeWebhookEvent evt, CancellationToken cancellationToken)
    {
        if (evt.StripeCustomerId is null) return;

        var subscription = await _dbContext.UserSubscription
            .FirstOrDefaultAsync(s => s.StripeCustomerId == evt.StripeCustomerId, cancellationToken);

        if (subscription is null) return;

        subscription.Update(new SubscriptionUpdateData(evt));
    }

    private async Task HandleSubscriptionDeletedAsync(StripeWebhookEvent evt, CancellationToken cancellationToken)
    {
        if (evt.StripeCustomerId is null) return;

        var subscription = await _dbContext.UserSubscription
            .FirstOrDefaultAsync(s => s.StripeCustomerId == evt.StripeCustomerId, cancellationToken);

        if (subscription is null) return;

        subscription.Update(new SubscriptionUpdateData(evt) with { StatusOverride = SubscriptionStatus.Canceled });
    }

    private async Task HandlePaymentFailedAsync(StripeWebhookEvent evt, CancellationToken cancellationToken)
    {
        if (evt.StripeCustomerId is null) return;

        var subscription = await _dbContext.UserSubscription
            .FirstOrDefaultAsync(s => s.StripeCustomerId == evt.StripeCustomerId, cancellationToken);

        if (subscription is null) return;

        subscription.Update(new SubscriptionUpdateData(evt) with { StatusOverride = SubscriptionStatus.PastDue });
    }
}

