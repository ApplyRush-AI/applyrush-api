using DTO.Attributes;

namespace DTO.Enums.Resume;

public enum ResumeAnalysisStatus
{
    [LocalizationKey("enum.resumeAnalysisStatus.notAnalyzed")]
    NotAnalyzed = 1,
    [LocalizationKey("enum.resumeAnalysisStatus.analyzing")]
    Analyzing = 2,
    [LocalizationKey("enum.resumeAnalysisStatus.completed")]
    Completed = 3,
    [LocalizationKey("enum.resumeAnalysisStatus.failed")]
    Failed = 4
}
