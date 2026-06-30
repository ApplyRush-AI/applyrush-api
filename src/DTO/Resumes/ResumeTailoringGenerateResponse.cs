namespace DTO.Resumes;

public sealed class ResumeTailoringGenerateResponse
{
    public decimal PreviousScore { get; init; }
    public decimal NewScore { get; init; }
    public IReadOnlyList<string> Changes { get; init; } = [];
    public int CreditsRemaining { get; init; }
}
