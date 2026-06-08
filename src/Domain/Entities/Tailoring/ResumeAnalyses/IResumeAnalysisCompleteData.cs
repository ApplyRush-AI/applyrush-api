using DTO.Enums.Resume;

namespace Domain.Entities.Tailoring.ResumeAnalyses;

public interface IResumeAnalysisCompleteData
{
    ResumeAnalysisGrade OverallGrade { get; }
    int UrgentFixCount { get; }
    int CriticalFixCount { get; }
    int OptionalFixCount { get; }
    string Issues { get; }
    ResumeAnalysisStatus Status { get; }
}
