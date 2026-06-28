using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Services.Configuration;

public sealed class StripeConfig
{
    public const string SectionName = "Stripe";

    [Required]
    public string SecretKey { get; set; } = null!;

    [Required]
    public string PublishableKey { get; set; } = null!;

    [Required]
    public string WebhookSecret { get; set; } = null!;

    [Required]
    public string CheckoutSuccessUrl { get; set; } = null!;

    [Required]
    public string CheckoutCancelUrl { get; set; } = null!;

    public StripePriceIds Prices { get; set; } = new();
}

public sealed class StripePriceIds
{
    public string ProMonthly { get; set; } = null!;
    public string ProQuarterly { get; set; } = null!;
    public string PremiumMonthly { get; set; } = null!;
    public string PremiumQuarterly { get; set; } = null!;
}
