namespace DTO.Resumes;

public sealed class CustomResumeResultResponse
{
    public TailoredResumeContent Content { get; init; } = new();
    public decimal ScoreBefore { get; init; }
    public decimal ScoreAfter { get; init; }
    public IReadOnlyList<string> Changes { get; init; } = [];
    public int CreditsRemaining { get; init; }
}
