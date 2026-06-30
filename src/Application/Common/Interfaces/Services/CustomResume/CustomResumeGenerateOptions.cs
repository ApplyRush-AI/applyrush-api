using DTO.Enums.Resume;

namespace Application.Common.Interfaces.Services.CustomResume;

public sealed record CustomResumeGenerateOptions
{
    public int UserId { get; init; }
    public int JobId { get; init; }
    public IReadOnlyList<string> SectionsToEnhance { get; init; } = [];
    public IReadOnlyList<string> KeywordsToInject { get; init; } = [];
    public TailoringWorkMode WorkMode { get; init; }
}
