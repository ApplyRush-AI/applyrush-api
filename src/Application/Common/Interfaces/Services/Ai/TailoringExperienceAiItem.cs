namespace Application.Common.Interfaces.Services.Ai;

public sealed record TailoringExperienceAiItem
{
    public int Id { get; init; }
    public IReadOnlyList<string> Bullets { get; init; } = null!;
}
