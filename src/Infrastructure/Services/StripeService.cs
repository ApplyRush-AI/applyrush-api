using Application.Common.Interfaces.Services;
using DTO.Enums.Subscription;
using DTO.Subscription;
using Infrastructure.Services.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services;

public sealed class StripeService : IStripeService
{
    private readonly StripeConfig _config;
    private readonly StripeClient _client;

    public StripeService(IOptions<StripeConfig> config)
    {
        _config = config.Value;
        _client = new StripeClient(_config.SecretKey);
    }

    public async Task<string> GetOrCreateCustomerAsync(int userId, string email, CancellationToken cancellationToken)
    {
        var customerService = new CustomerService(_client);

        var existing = await customerService.ListAsync(new CustomerListOptions
        {
            Email = email,
            Limit = 1
        }, cancellationToken: cancellationToken);

        if (existing.Data.Count > 0)
            return existing.Data[0].Id;

        var customer = await customerService.CreateAsync(new CustomerCreateOptions
        {
            Email = email,
            Metadata = new Dictionary<string, string>
            {
                { "userId", userId.ToString() }
            }
        }, cancellationToken: cancellationToken);

        return customer.Id;
    }

    public async Task<string> CreateCheckoutSessionAsync(string stripeCustomerId, SubscriptionPlan plan, BillingInterval interval, CancellationToken cancellationToken)
    {
        var priceId = ResolvePrice(plan, interval);

        var sessionService = new SessionService(_client);

        var session = await sessionService.CreateAsync(new SessionCreateOptions
        {
            Customer = stripeCustomerId,
            Mode = "subscription",
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions { Price = priceId, Quantity = 1 }
            },
            SuccessUrl = _config.CheckoutSuccessUrl,
            CancelUrl = _config.CheckoutCancelUrl
        }, cancellationToken: cancellationToken);

        return session.Url;
    }

    public async Task CancelSubscriptionAtPeriodEndAsync(string stripeSubscriptionId, CancellationToken cancellationToken)
    {
        var subscriptionService = new SubscriptionService(_client);

        await subscriptionService.UpdateAsync(stripeSubscriptionId, new SubscriptionUpdateOptions
        {
            CancelAtPeriodEnd = true
        }, cancellationToken: cancellationToken);
    }

    public async Task<PaymentMethodResponse?> GetPaymentMethodAsync(string stripeCustomerId, CancellationToken cancellationToken)
    {
        var paymentMethodService = new PaymentMethodService(_client);

        var methods = await paymentMethodService.ListAsync(new PaymentMethodListOptions
        {
            Customer = stripeCustomerId,
            Type = "card",
            Limit = 1
        }, cancellationToken: cancellationToken);

        var pm = methods.Data.FirstOrDefault();
        if (pm?.Card is null) return null;

        return new PaymentMethodResponse(
            pm.Card.Last4,
            pm.Card.Brand,
            (int)pm.Card.ExpMonth,
            (int)pm.Card.ExpYear
        );
    }

    public async Task<IReadOnlyList<InvoiceResponse>> GetInvoicesAsync(string stripeCustomerId, CancellationToken cancellationToken)
    {
        var invoiceService = new InvoiceService(_client);

        var invoices = await invoiceService.ListAsync(new InvoiceListOptions
        {
            Customer = stripeCustomerId,
            Limit = 24
        }, cancellationToken: cancellationToken);

        return invoices.Data
            .Select(inv => new InvoiceResponse(
                inv.Id,
                inv.Created,
                inv.AmountPaid / 100m,
                inv.Status ?? "unknown",
                inv.HostedInvoiceUrl ?? string.Empty
            ))
            .ToList();
    }

    public Task<Application.Common.Interfaces.Services.StripeWebhookEvent> ParseWebhookEventAsync(string rawPayload, string signature, CancellationToken cancellationToken)
    {
        var stripeEvent = EventUtility.ConstructEvent(rawPayload, signature, _config.WebhookSecret);

        string? subscriptionId = null;
        string? customerId = null;
        SubscriptionPlan? plan = null;
        BillingInterval? interval = null;
        SubscriptionStatus? status = null;
        DateTime? currentPeriodStart = null;
        DateTime? currentPeriodEnd = null;
        DateTime? canceledAt = null;

        if (stripeEvent.Data.Object is Stripe.Subscription subscription)
        {
            subscriptionId = subscription.Id;
            customerId = subscription.CustomerId;
            status = MapStatus(subscription.Status);
            canceledAt = subscription.CanceledAt;

            // Note: Stripe.net v51 removed CurrentPeriodStart/End from Subscription.
            // These are not exposed in the top-level entity; pass null until the SDK re-exposes them.
            currentPeriodStart = null;
            currentPeriodEnd = null;

            var priceId = subscription.Items.Data.FirstOrDefault()?.Price?.Id;
            (plan, interval) = ResolvePlanFromPriceId(priceId);
        }
        else if (stripeEvent.Data.Object is Session session)
        {
            customerId = session.CustomerId;
        }

        return Task.FromResult(new Application.Common.Interfaces.Services.StripeWebhookEvent(
            stripeEvent.Type,
            subscriptionId,
            customerId,
            plan,
            interval,
            status,
            currentPeriodStart,
            currentPeriodEnd,
            canceledAt
        ));
    }

    private string ResolvePrice(SubscriptionPlan plan, BillingInterval interval) =>
        (plan, interval) switch
        {
            (SubscriptionPlan.Pro, BillingInterval.Monthly) => _config.Prices.ProMonthly,
            (SubscriptionPlan.Pro, BillingInterval.Quarterly) => _config.Prices.ProQuarterly,
            (SubscriptionPlan.Premium, BillingInterval.Monthly) => _config.Prices.PremiumMonthly,
            (SubscriptionPlan.Premium, BillingInterval.Quarterly) => _config.Prices.PremiumQuarterly,
            _ => throw new ArgumentOutOfRangeException(nameof(plan), "Invalid plan/interval combination.")
        };

    private (SubscriptionPlan? Plan, BillingInterval? Interval) ResolvePlanFromPriceId(string? priceId)
    {
        if (priceId is null) return (null, null);

        if (priceId == _config.Prices.ProMonthly) return (SubscriptionPlan.Pro, BillingInterval.Monthly);
        if (priceId == _config.Prices.ProQuarterly) return (SubscriptionPlan.Pro, BillingInterval.Quarterly);
        if (priceId == _config.Prices.PremiumMonthly) return (SubscriptionPlan.Premium, BillingInterval.Monthly);
        if (priceId == _config.Prices.PremiumQuarterly) return (SubscriptionPlan.Premium, BillingInterval.Quarterly);

        return (null, null);
    }

    private static SubscriptionStatus MapStatus(string? status) =>
        status switch
        {
            "active" => SubscriptionStatus.Active,
            "past_due" => SubscriptionStatus.PastDue,
            "canceled" => SubscriptionStatus.Canceled,
            "trialing" => SubscriptionStatus.Trialing,
            "incomplete" or "incomplete_expired" => SubscriptionStatus.Incomplete,
            _ => SubscriptionStatus.Active
        };
}



