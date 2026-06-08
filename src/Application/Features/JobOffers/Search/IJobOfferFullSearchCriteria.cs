using Application.Common.Search;
using DTO.Enums.Job;
using DTO.Enums.JobOffer;

namespace Application.Features.JobOffers.Search;

public interface IJobOfferFullSearchCriteria : IFullSearchCriteria<JobOfferFeedSortField>
{
    new string? Query { get; }
    JobType[]? JobTypes { get; }
    WorkModel? WorkModel { get; }
    ExperienceLevel? ExperienceLevel { get; }
    decimal? SalaryMin { get; }
    string? Industry { get; }
}
