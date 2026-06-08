using Application.Features.JobOffers.Models;

namespace Application.Common.Interfaces.Services;

public interface IJobProvider
{
    Task<IReadOnlyList<JobListingData>> FetchJobsAsync(string query, int page, CancellationToken cancellationToken);
}
