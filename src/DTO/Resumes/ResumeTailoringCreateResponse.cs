using DTO.Response;

namespace DTO.Resumes;

public sealed class ResumeTailoringCreateResponse
{
    public int TailoringId { get; init; }
    public decimal ScoreBefore { get; init; }
    public decimal ScoreAfter { get; init; }
    public string? Summary { get; init; }
    public IReadOnlyList<TailoringExperienceResponse> Experience { get; init; } = [];
    public IReadOnlyList<string> HighlightedSkills { get; init; } = [];
    public IReadOnlyList<string> MissingSkills { get; init; } = [];
    public int CreditsRemaining { get; init; }
    public ListItemBaseResponse Status { get; init; } = null!;
}
