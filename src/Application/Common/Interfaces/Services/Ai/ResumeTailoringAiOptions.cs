namespace Application.Common.Interfaces.Services.Ai;

public sealed record ResumeTailoringAiOptions
{
    public int UserId { get; init; }
    public int JobId { get; init; }
    public IReadOnlyList<string> SectionsToEnhance { get; init; } = null!;
    public IReadOnlyList<string> KeywordsToInject { get; init; } = null!;
}
