using DTO.JobOffers;

namespace DTO.Resumes;

public sealed record CustomResumeGapAnalysisResponse
{
    public decimal MatchScore { get; init; }
    public MatchScoresResponse Scores { get; init; } = null!;
    public IReadOnlyList<string> MatchedSkills { get; init; } = [];
    public IReadOnlyList<string> MissingSkills { get; init; } = [];
}
