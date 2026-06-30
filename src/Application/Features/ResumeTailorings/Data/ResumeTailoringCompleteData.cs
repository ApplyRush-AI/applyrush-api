using Domain.Entities.Tailoring.ResumeTailorings;
using DTO.Enums.Resume;

namespace Application.Features.ResumeTailorings.Data;

internal sealed record ResumeTailoringCompleteData : IResumeTailoringCompleteData
{
    public string TailoredContent { get; init; } = null!;
    public decimal ScoreBefore { get; init; }
    public decimal ScoreAfter { get; init; }
    public TailoringStatus Status { get; init; }
    public IReadOnlyList<string> Changes { get; init; } = [];
}
