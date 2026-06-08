namespace DTO.JobOffers;

public record JobOfferDescriptionResponse
{
    public string? About { get; init; }
    public IReadOnlyList<string> Responsibilities { get; init; } = [];
    public IReadOnlyList<string> Requirements { get; init; } = [];
    public IReadOnlyList<string> Benefits { get; init; } = [];
}
