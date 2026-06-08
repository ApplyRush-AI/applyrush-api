namespace DTO.JobOffers;

public record MatchScoresResponse
{
    public decimal Experience { get; init; }
    public decimal Skills { get; init; }
    public decimal Title { get; init; }
    public decimal Industry { get; init; }
}
