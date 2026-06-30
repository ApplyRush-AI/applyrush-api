using DTO.Response;

namespace DTO.JobOffers;

public record JobOfferFeedItemResponse
{
    public int Id { get; init; }
    public string Company { get; init; } = null!;
    public string CompanyShort { get; init; } = null!;
    public string? LogoColor { get; init; }
    public string Title { get; init; } = null!;
    public string ApplyUrl { get; init; } = null!;
    public string Location { get; init; } = null!;
    public ListItemBaseResponse WorkModel { get; init; } = null!;
    public ListItemBaseResponse JobType { get; init; } = null!;
    public ListItemBaseResponse ExperienceLevel { get; init; } = null!;
    public string? Industry { get; init; }
    public bool H1BSupported { get; init; }
    public IReadOnlyList<ListItemBaseResponse> JobFunctions { get; init; } = [];
    public int? YearsRequired { get; init; }
    public string? Salary { get; init; }
    public IReadOnlyList<string> RequiredSkills { get; init; } = [];
    public decimal? SalaryMin { get; init; }
    public decimal? SalaryMax { get; init; }
    public DateTime PostedAt { get; init; }
    public string PostedAgo { get; init; } = null!;
    public decimal? MatchScore { get; init; }
    public MatchScoresResponse? Scores { get; init; }
    public IReadOnlyList<string> MatchedSkills { get; init; } = [];
    public ListItemBaseResponse? MatchTier { get; init; }
    public bool IsSaved { get; init; }
    public bool IsHidden { get; init; }
}
