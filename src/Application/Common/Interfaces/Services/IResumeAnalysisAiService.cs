using Application.Common.Interfaces.Services.Ai;

namespace Application.Common.Interfaces.Services;

public interface IResumeAnalysisAiService
{
    Task<ResumeAnalysisAiResult> AnalyzeAsync(ResumeAnalysisAiOptions options, CancellationToken cancellationToken);
    Task<ResumeAnalysisFixAiResult> FixIssueAsync(ResumeAnalysisFixAiOptions options, CancellationToken cancellationToken);
    Task<IReadOnlyList<ResumeAnalysisFixAiResult>> FixAllIssuesAsync(ResumeAnalysisFixAllAiOptions options, CancellationToken cancellationToken);
}
