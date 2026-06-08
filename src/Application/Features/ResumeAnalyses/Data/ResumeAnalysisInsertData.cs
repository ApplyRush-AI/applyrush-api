using Domain.Entities.Tailoring.ResumeAnalyses;

namespace Application.Features.ResumeAnalyses.Data;

internal sealed record ResumeAnalysisInsertData(int UserId, int CreditsUsed) : IResumeAnalysisInsertData;
