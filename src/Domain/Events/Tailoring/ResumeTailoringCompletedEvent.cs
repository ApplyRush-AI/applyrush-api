using Domain.Entities.Tailoring.ResumeTailorings;

namespace Domain.Events.Tailoring;

public sealed class ResumeTailoringCompletedEvent : BaseEvent
{
    public ResumeTailoringCompletedEvent(ResumeTailoring tailoring)
    {
        Tailoring = tailoring;
    }

    public ResumeTailoring Tailoring { get; }
}
