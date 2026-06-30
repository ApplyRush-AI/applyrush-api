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
    decimal? SalaryMax { get; }
    string? Industry { get; }
    int[]? JobFunctionIds { get; }
    int? MinYearsOfExperience { get; }
    int? MaxYearsOfExperience { get; }
    string? Location { get; }
    DateTime? PostedAfter { get; }
    DateTime? PostedBefore { get; }
    string[]? Skills { get; }
    int[]? ExcludeIds { get; }
    int[]? IncludeIds { get; }
}
