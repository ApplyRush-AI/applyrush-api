using DTO.Enums.Resume;

namespace Domain.Entities.Tailoring.ResumeAnalyses;

public interface IResumeAnalysisInsertData
{
    int UserId { get; }
    int CreditsUsed { get; }
}
