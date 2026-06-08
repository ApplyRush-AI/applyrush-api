using Domain.Entities.Tailoring.ResumeTailorings;

namespace Domain.Events.Tailoring;

public sealed class ResumeTailoringCreatedEvent : BaseEvent
{
    public ResumeTailoringCreatedEvent(ResumeTailoring tailoring)
    {
        Tailoring = tailoring;
    }

    public ResumeTailoring Tailoring { get; }
}
