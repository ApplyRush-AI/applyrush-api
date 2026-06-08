namespace DTO.JobOffers;

public sealed record JobOfferDetailResponse : JobOfferFeedItemResponse
{
    public string ApplyUrl { get; init; } = null!;
    public int? YearsRequired { get; init; }
    public int? ApplicantCount { get; init; }
    public IReadOnlyList<string> RequiredSkills { get; init; } = [];
    public string? AiSummary { get; init; }
    public JobOfferDescriptionResponse Description { get; init; } = null!;
}
