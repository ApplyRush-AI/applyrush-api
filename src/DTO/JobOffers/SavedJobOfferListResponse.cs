namespace DTO.JobOffers;

public sealed record SavedJobOfferListResponse
{
    public IReadOnlyList<int> SavedJobIds { get; init; } = [];
    public IReadOnlyList<JobOfferFeedItemResponse> Jobs { get; init; } = [];
}
