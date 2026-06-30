using Application.Common.Search;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.JobOffers.Search;
using Domain.Entities.Jobs.UserJobMatches;
using DTO.Enums.Job;
using DTO.Enums.JobOffer;
using DTO.JobOffers;
using DTO.Pagination;
using DTO.Response;
using DTO.Sorting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Application.Features.JobOffers.Queries;

public sealed record JobOfferFeedQuery(
    string? Query,
    JobType[]? JobTypes,
    WorkModel? WorkModel,
    ExperienceLevel? ExperienceLevel,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string? Industry,
    int[]? JobFunctionIds,
    int? MinYearsOfExperience,
    int? MaxYearsOfExperience,
    string? Location,
    DateTime? PostedAfter,
    DateTime? PostedBefore,
    string[]? Skills,
    bool? Hidden,
    PaginationOptions Paging,
    SortOptions<JobOfferFeedSortField>? Sorting)
    : IJobOfferFullSearchCriteria, IQuery<PaginatedList<JobOfferSearchable>>
{
    public int[]? ExcludeIds { get; init; }
    public int[]? IncludeIds { get; init; }
}

public sealed class JobOfferFeedQueryHandler : IQueryHandler<JobOfferFeedQuery, PaginatedList<JobOfferSearchable>>
{
    private readonly ISearchClient<JobOfferSearchable> _searchClient;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public JobOfferFeedQueryHandler(
        ISearchClient<JobOfferSearchable> searchClient,
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _searchClient = searchClient;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedList<JobOfferSearchable>> Handle(JobOfferFeedQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var hiddenJobIds = await LoadHiddenJobIdsAsync(userId, cancellationToken);
        var criteria = ApplyHiddenFilter(request, hiddenJobIds);

        var result = userId.HasValue && criteria.Sorting?.Field == JobOfferFeedSortField.Match
            ? await GetFeedSortedByMatchScoreAsync(criteria, userId.Value, hiddenJobIds, cancellationToken)
            : await _searchClient.SearchJobOffersAsync(criteria);

        result = await MergeMatchScoresAsync(result, userId, request.Paging.PageSize, cancellationToken);

        return userId.HasValue && request.Hidden == true
            ? MarkAllAsHidden(result, request.Paging.PageSize)
            : result;
    }

    private async Task<int[]> LoadHiddenJobIdsAsync(int? userId, CancellationToken cancellationToken)
    {
        if (!userId.HasValue) return [];
        return await _dbContext.UserHiddenJob
            .Where(h => h.UserId == userId.Value)
            .Select(h => h.JobId)
            .ToArrayAsync(cancellationToken);
    }

    private static JobOfferFeedQuery ApplyHiddenFilter(JobOfferFeedQuery request, int[] hiddenJobIds)
    {
        if (hiddenJobIds.Length == 0) return request;
        return request.Hidden == true
            ? request with { IncludeIds = hiddenJobIds }
            : request with { ExcludeIds = hiddenJobIds };
    }

    private static PaginatedList<JobOfferSearchable> MarkAllAsHidden(PaginatedList<JobOfferSearchable> result, int pageSize)
    {
        var items = result.Items.Select(i => i with { IsHidden = true }).ToList();
        return new PaginatedList<JobOfferSearchable>(items, result.TotalCount, result.PageNumber, pageSize);
    }

    private async Task<PaginatedList<JobOfferSearchable>> GetFeedSortedByMatchScoreAsync(
        JobOfferFeedQuery criteria, int userId, int[] hiddenJobIds, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(1, criteria.Paging.PageNumber);
        var pageSize = Math.Max(1, criteria.Paging.PageSize);
        var descending = criteria.Sorting!.SortOrder != SortOrder.Asc;

        var matchQuery = _dbContext.UserJobMatch
            .AsNoTracking()
            .Where(m => m.UserId == userId);

        if (hiddenJobIds.Length > 0)
        {
            matchQuery = criteria.Hidden == true
                ? matchQuery.Where(m => hiddenJobIds.Contains(m.JobId))
                : matchQuery.Where(m => !hiddenJobIds.Contains(m.JobId));
        }

        var totalCount = await matchQuery.CountAsync(cancellationToken);

        var pagedMatches = await (descending
                ? matchQuery.OrderByDescending(m => m.OverallScore)
                : matchQuery.OrderBy(m => m.OverallScore))
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        if (pagedMatches.Count == 0)
            return new PaginatedList<JobOfferSearchable>([], totalCount, pageNumber, pageSize);

        var jobIds = pagedMatches.Select(m => m.JobId).ToList();
        var esItems = await _searchClient.GetJobOffersByIdsAsync(jobIds, cancellationToken);
        var esByJobId = esItems.ToDictionary(i => i.Id);
        var matchByJobId = pagedMatches.ToDictionary(m => m.JobId);

        var items = jobIds
            .Where(esByJobId.ContainsKey)
            .Select(jobId => ApplyMatchScore(esByJobId[jobId], matchByJobId[jobId]))
            .ToList();

        return new PaginatedList<JobOfferSearchable>(items, totalCount, pageNumber, pageSize);
    }

    private async Task<PaginatedList<JobOfferSearchable>> MergeMatchScoresAsync(
        PaginatedList<JobOfferSearchable> result, int? userId, int pageSize, CancellationToken cancellationToken)
    {
        if (!userId.HasValue || result.Items.Count == 0)
            return result;

        var jobIds = result.Items.Select(i => i.Id).ToList();
        var matches = await _dbContext.UserJobMatch
            .AsNoTracking()
            .Where(m => m.UserId == userId.Value && jobIds.Contains(m.JobId))
            .ToListAsync(cancellationToken);

        if (matches.Count == 0)
            return result;

        var matchByJobId = matches.ToDictionary(m => m.JobId);
        var enrichedItems = result.Items
            .Select(item => matchByJobId.TryGetValue(item.Id, out var match) ? ApplyMatchScore(item, match) : item)
            .ToList();

        return new PaginatedList<JobOfferSearchable>(enrichedItems, result.TotalCount, result.PageNumber, pageSize);
    }

    private static JobOfferSearchable ApplyMatchScore(JobOfferSearchable item, UserJobMatch match)
    {
        IReadOnlyList<string> matchedSkills = string.IsNullOrEmpty(match.MatchedSkillsJson)
            ? []
            : JsonSerializer.Deserialize<List<string>>(match.MatchedSkillsJson) ?? [];

        return item with
        {
            MatchScore = match.OverallScore,
            Scores = new MatchScoresResponse
            {
                Experience = match.ExperienceScore,
                Skills = match.SkillScore,
                Title = match.TitleScore,
                Industry = match.IndustryScore
            },
            MatchedSkills = matchedSkills,
            MatchTier = new ListItemBaseResponse { Id = (int)match.MatchTier, Name = match.MatchTier.ToString() }
        };
    }
}
