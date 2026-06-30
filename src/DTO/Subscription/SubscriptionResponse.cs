using DTO.Response;

namespace DTO.Subscription;

public sealed record SubscriptionResponse
{
    public ListItemBaseResponse Plan { get; init; } = null!;
    public ListItemBaseResponse Status { get; init; } = null!;
    public ListItemBaseResponse? BillingInterval { get; init; }
    public DateTime? RenewalDate { get; init; }
    public string Price { get; init; } = null!;
    public DateTime? CanceledAt { get; init; }
}
