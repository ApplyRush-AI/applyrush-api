using DTO.Enums.Job;

namespace Domain.Entities.Jobs.JobListings;

public interface IJobListingInsertData : IJobListingBaseData
{
    string ExternalId { get; }
    JobSource Source { get; }
}
