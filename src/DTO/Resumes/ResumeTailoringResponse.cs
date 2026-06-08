using DTO.Response;

namespace DTO.Resumes;

public sealed class ResumeTailoringResponse
{
    public int Id { get; init; }
    public int JobId { get; init; }
    public int? ResumeId { get; init; }
    public decimal ScoreBefore { get; init; }
    public decimal ScoreAfter { get; init; }
    public string TailoredContent { get; init; } = null!;
    public ListItemBaseResponse Status { get; init; } = null!;
    public DateTime DateCreated { get; init; }
}
