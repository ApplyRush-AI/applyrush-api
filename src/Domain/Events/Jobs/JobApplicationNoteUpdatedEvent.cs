using Domain.Entities.Jobs.JobApplications;

namespace Domain.Events.Jobs;

public sealed class JobApplicationNoteUpdatedEvent : BaseEvent
{
    public JobApplicationNoteUpdatedEvent(JobApplication application)
    {
        Application = application;
    }

    public JobApplication Application { get; }
}
