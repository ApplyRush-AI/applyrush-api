using Domain.Entities.Tailoring.ResumeAnalyses;

namespace Domain.Events.Tailoring;

public sealed class ResumeAnalysisCreatedEvent : BaseEvent
{
    public ResumeAnalysisCreatedEvent(ResumeAnalysis analysis)
    {
        Analysis = analysis;
    }

    public ResumeAnalysis Analysis { get; }
}
