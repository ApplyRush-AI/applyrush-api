namespace Application.Common.Interfaces.Services.Ai;

public sealed record ResumeTailoringAiRewriteResult
{
    public string Section { get; init; } = null!;
    public string UpdatedContent { get; init; } = null!;
}
