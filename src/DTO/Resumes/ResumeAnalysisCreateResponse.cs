using DTO.Response;

namespace DTO.Resumes;

public sealed class ResumeAnalysisCreateResponse
{
    public int Id { get; init; }
    public ListItemBaseResponse Status { get; init; } = null!;
    public int CreditsRemaining { get; init; }
}
