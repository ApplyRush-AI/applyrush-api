namespace DTO.JobOffers;

public sealed record JobOfferDetailResponse : JobOfferFeedItemResponse
{
    public int? ApplicantCount { get; init; }
    public string? AiSummary { get; init; }
    public JobOfferDescriptionResponse Description { get; init; } = null!;
}
