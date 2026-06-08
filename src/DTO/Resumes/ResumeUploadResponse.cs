namespace DTO.Resumes;

public sealed record ResumeUploadResponse
{
    public int ResumeId { get; init; }
    public ResumeParseDataResponse ParsedData { get; init; } = null!;
}
