namespace Application.Common.Interfaces.Services.Ai;

public sealed record ExtensionAnswerAiOptions
{
    public string Question { get; init; } = null!;
    public string? JobDescription { get; init; }
    public string? UserTitle { get; init; }
    public IReadOnlyList<string> Skills { get; init; } = [];
    public string? Summary { get; init; }
}
