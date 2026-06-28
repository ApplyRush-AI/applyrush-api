using Domain.Entities.Jobs.JobListings;

namespace Domain.Events.Jobs;

public sealed class JobListingCreatedEvent : BaseEvent
{
    public JobListingCreatedEvent(JobListing jobListing)
    {
        JobListing = jobListing;
    }

    public JobListing JobListing { get; }
}
