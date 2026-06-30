namespace Application.Common.Interfaces.Services.Ai;

public sealed record ResumeTailoringAiResult
{
    public string TailoredContent { get; init; } = null!;
    public decimal ScoreBefore { get; init; }
    public decimal ScoreAfter { get; init; }
    public string? Summary { get; init; }
    public IReadOnlyList<TailoringExperienceAiItem> Experience { get; init; } = null!;
    public IReadOnlyList<string> HighlightedSkills { get; init; } = null!;
    public IReadOnlyList<string> MissingSkills { get; init; } = null!;
    public IReadOnlyList<string> Changes { get; init; } = [];
}
