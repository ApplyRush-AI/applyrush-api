using DTO.Enums.Resume;

namespace Domain.Entities.Tailoring.ResumeTailorings;

public interface IResumeTailoringCompleteData
{
    string TailoredContent { get; }
    decimal ScoreBefore { get; }
    decimal ScoreAfter { get; }
    TailoringStatus Status { get; }
}
