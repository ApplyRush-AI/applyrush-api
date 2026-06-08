using Application.Common.Search;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.JobOffers.Search;
using DTO.Enums.Job;
using DTO.Enums.JobOffer;
using DTO.Pagination;
using DTO.Sorting;

namespace Application.Features.JobOffers.Queries;

public sealed record JobOfferFeedQuery(
    string? Query,
    JobType[]? JobTypes,
    WorkModel? WorkModel,
    ExperienceLevel? ExperienceLevel,
    decimal? SalaryMin,
    string? Industry,
    PaginationOptions Paging,
    SortOptions<JobOfferFeedSortField>? Sorting)
    : IJobOfferFullSearchCriteria, IQuery<PaginatedList<JobOfferSearchable>>;

public sealed class JobOfferFeedQueryHandler : IQueryHandler<JobOfferFeedQuery, PaginatedList<JobOfferSearchable>>
{
    private readonly ISearchClient<JobOfferSearchable> _searchClient;

    public JobOfferFeedQueryHandler(ISearchClient<JobOfferSearchable> searchClient)
    {
        _searchClient = searchClient;
    }

    public async Task<PaginatedList<JobOfferSearchable>> Handle(JobOfferFeedQuery query, CancellationToken cancellationToken)
    {
        return await _searchClient.SearchJobOffersAsync(query);
    }
}
