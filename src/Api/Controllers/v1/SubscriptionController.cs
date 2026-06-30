using Application.Features.Subscriptions.Commands;
using Application.Features.Subscriptions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

[Route("api/v{version:apiVersion}/billing")]
public class SubscriptionController : ApiControllerBase
{
    [HttpGet("subscription")]
    public async Task<IActionResult> GetCurrentSubscription()
    {
        return Ok(await Mediator.Send(new SubscriptionGetCurrentQuery()));
    }

    [HttpGet("payment-method")]
    public async Task<IActionResult> GetPaymentMethod()
    {
        return Ok(await Mediator.Send(new PaymentMethodGetQuery()));
    }

    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoices()
    {
        return Ok(await Mediator.Send(new InvoiceListGetQuery()));
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] SubscriptionCheckoutCreateCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPost("checkout/confirm")]
    public async Task<IActionResult> ConfirmCheckout([FromBody] SubscriptionConfirmCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> Cancel()
    {
        return Ok(await Mediator.Send(new SubscriptionCancelCommand()));
    }

    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        using var reader = new StreamReader(Request.Body);
        var rawPayload = await reader.ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"].ToString();

        await Mediator.Send(new SubscriptionWebhookHandleCommand(signature, rawPayload));
        return Ok();
    }
}
