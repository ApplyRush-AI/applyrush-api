using DTO.Resumes;

namespace Application.Common.Interfaces.Services.CustomResume;

public sealed record CustomResumeAiResult
{
    public TailoredResumeContent Content { get; init; } = new();
    public decimal ScoreBefore { get; init; }
    public decimal ScoreAfter { get; init; }
    public IReadOnlyList<string> Changes { get; init; } = [];
}
