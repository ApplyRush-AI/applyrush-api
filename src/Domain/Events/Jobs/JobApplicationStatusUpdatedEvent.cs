using Domain.Entities.Jobs.JobApplications;

namespace Domain.Events.Jobs;

public sealed class JobApplicationStatusUpdatedEvent : BaseEvent
{
    public JobApplicationStatusUpdatedEvent(JobApplication application)
    {
        Application = application;
    }

    public JobApplication Application { get; }
}
