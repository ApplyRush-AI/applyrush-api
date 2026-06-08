using Application.Common.Interfaces.Services.Ai;

namespace Application.Common.Interfaces.Services;

public interface IResumeTailoringAiService
{
    Task<ResumeTailoringAiResult> TailorAsync(ResumeTailoringAiOptions options, CancellationToken cancellationToken);
    Task<ResumeTailoringAiRewriteResult> RewriteAsync(ResumeTailoringAiRewriteOptions options, CancellationToken cancellationToken);
}
