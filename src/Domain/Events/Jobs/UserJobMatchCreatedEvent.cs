namespace Domain.Events.Jobs;

public sealed class UserJobMatchCreatedEvent : BaseEvent
{
    public UserJobMatchCreatedEvent(int jobId)
    {
        JobId = jobId;
    }

    public int JobId { get; }
}
