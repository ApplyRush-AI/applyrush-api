using DTO.Enums.Job;

namespace DTO.JobOffers;

public sealed record JobOfferSyncStatusResponse
{
    public JobSyncStatus Status { get; init; }
    public DateTime? LastSyncedAt { get; init; }
    public int JobsIndexed { get; init; }
    public string? ErrorMessage { get; init; }
}
