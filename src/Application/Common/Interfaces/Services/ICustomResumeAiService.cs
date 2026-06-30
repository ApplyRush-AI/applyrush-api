using Application.Common.Interfaces.Services.CustomResume;
using DTO.Enums.Resume;
using DTO.Resumes;

namespace Application.Common.Interfaces.Services;

public interface ICustomResumeAiService
{
    Task<CustomResumeAiResult> GenerateAsync(CustomResumeGenerateOptions options, CancellationToken cancellationToken);
    Task<TailoredResumeContent> RewriteAsync(TailoredResumeContent current, AiRewriteAction action, string? freeFormInstruction, CancellationToken cancellationToken);
}
