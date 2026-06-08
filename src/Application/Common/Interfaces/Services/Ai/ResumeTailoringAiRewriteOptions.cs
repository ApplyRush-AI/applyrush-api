using DTO.Enums.Resume;

namespace Application.Common.Interfaces.Services.Ai;

public sealed record ResumeTailoringAiRewriteOptions
{
    public int TailoringId { get; init; }
    public AiRewriteAction Action { get; init; }
    public string? FreeFormInstruction { get; init; }
}
