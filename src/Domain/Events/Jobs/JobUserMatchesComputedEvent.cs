using Domain.Entities.Jobs.JobListings;

namespace Domain.Events.Jobs;

public sealed class JobUserMatchesComputedEvent : BaseEvent
{
    public JobUserMatchesComputedEvent(JobListing jobListing)
    {
        JobListing = jobListing;
    }

    public JobListing JobListing { get; }
}
