using DTO.Response;

namespace DTO.JobOffers;

public record JobOfferFeedItemResponse
{
    public int Id { get; init; }
    public string Company { get; init; } = null!;
    public string CompanyShort { get; init; } = null!;
    public string? LogoColor { get; init; }
    public string Title { get; init; } = null!;
    public string Location { get; init; } = null!;
    public ListItemBaseResponse WorkModel { get; init; } = null!;
    public ListItemBaseResponse JobType { get; init; } = null!;
    public ListItemBaseResponse ExperienceLevel { get; init; } = null!;
    public string? Industry { get; init; }
    public string? Salary { get; init; }
    public string PostedAgo { get; init; } = null!;
    public decimal? MatchScore { get; init; }
    public MatchScoresResponse? Scores { get; init; }
    public IReadOnlyList<string> MatchedSkills { get; init; } = [];
    public ListItemBaseResponse? MatchTier { get; init; }
    public bool IsSaved { get; init; }
}
