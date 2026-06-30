using DTO.Enums.Subscription;
using DTO.Subscription;

namespace Application.Common.Interfaces.Services;

public interface IStripeService
{
    /// <summary>Creates or retrieves the Stripe customer for a user.</summary>
    Task<string> GetOrCreateCustomerAsync(int userId, string email, CancellationToken cancellationToken);

    /// <summary>Creates a Stripe Checkout Session and returns the URL.</summary>
    Task<string> CreateCheckoutSessionAsync(string stripeCustomerId, SubscriptionPlan plan, BillingInterval interval, CancellationToken cancellationToken);

    /// <summary>Retrieves a completed Checkout Session and its subscription details for confirmation.</summary>
    Task<StripeWebhookEvent> GetCheckoutSessionAsync(string sessionId, CancellationToken cancellationToken);

    /// <summary>Cancels the Stripe subscription at the current period end.</summary>
    Task CancelSubscriptionAtPeriodEndAsync(string stripeSubscriptionId, CancellationToken cancellationToken);

    /// <summary>Gets the default payment method for a customer.</summary>
    Task<PaymentMethodResponse?> GetPaymentMethodAsync(string stripeCustomerId, CancellationToken cancellationToken);

    /// <summary>Gets the invoice list for a customer.</summary>
    Task<IReadOnlyList<InvoiceResponse>> GetInvoicesAsync(string stripeCustomerId, CancellationToken cancellationToken);

    /// <summary>Verifies a Stripe webhook signature and returns the event payload.</summary>
    Task<StripeWebhookEvent> ParseWebhookEventAsync(string rawPayload, string signature, CancellationToken cancellationToken);
}
