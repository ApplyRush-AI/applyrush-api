using Domain.Entities.Jobs.JobApplications;

namespace Domain.Events.Jobs;

public sealed class JobApplicationCreatedEvent : BaseEvent
{
    public JobApplicationCreatedEvent(JobApplication application)
    {
        Application = application;
    }

    public JobApplication Application { get; }
}
