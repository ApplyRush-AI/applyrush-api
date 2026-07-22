namespace DTO.Search;

public sealed record SearchIndexRebuildResult
{
    public bool Succeeded { get; init; }
    public string Index { get; init; } = null!;
    public int DocumentsIndexed { get; init; }
    public string? Error { get; init; }
}
