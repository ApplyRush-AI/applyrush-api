using DTO.JobOffers;
using DTO.Response;

namespace DTO.JobApplications;

public sealed record JobApplicationResponse
{
    public int JobId { get; init; }
    public ListItemBaseResponse Stage { get; init; } = null!;
    public DateTime DateCreated { get; init; }
    public string? Note { get; init; }
    public JobOfferFeedItemResponse? Job { get; init; }
}
