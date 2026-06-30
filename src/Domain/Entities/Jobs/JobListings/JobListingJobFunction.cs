using Domain.Entities.Base;
using Domain.Entities.JobFunctions;

namespace Domain.Entities.Jobs.JobListings;

public sealed class JobListingJobFunction : BaseEntity
{
    private JobListingJobFunction() { }

    public int JobListingId { get; private set; }
    public int JobFunctionId { get; private set; }

    public JobListing JobListing { get; } = null!;
    public JobFunction JobFunction { get; } = null!;

    public static JobListingJobFunction Create(int jobFunctionId) => new()
    {
        JobFunctionId = jobFunctionId
    };
}
