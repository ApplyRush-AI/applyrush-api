namespace Application.Common.Interfaces.Services.Ai;

public sealed record ExtensionAnswerAiResult
{
    public string Answer { get; init; } = null!;
}
