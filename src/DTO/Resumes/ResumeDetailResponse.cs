using DTO.Response;

namespace DTO.Resumes;

public sealed record ResumeDetailResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public bool IsPrimary { get; init; }
    public ListItemBaseResponse FileType { get; init; } = null!;
    public ListItemBaseResponse ParseStatus { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime LastModifiedAt { get; init; }
    public ResumeParseDataResponse? ParsedData { get; init; }
}
