using Domain.Entities.Tailoring.ResumeAnalyses;

namespace Domain.Events.Tailoring;

public sealed class ResumeAnalysisCompletedEvent : BaseEvent
{
    public ResumeAnalysisCompletedEvent(ResumeAnalysis analysis)
    {
        Analysis = analysis;
    }

    public ResumeAnalysis Analysis { get; }
}
