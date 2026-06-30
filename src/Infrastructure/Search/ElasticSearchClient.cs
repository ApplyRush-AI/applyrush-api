using Application.Common.Search;
using Application.Features.JobOffers.Search;
using Application.Features.Notifications.Search;
using Application.Features.Users.Search;
using DTO.Enums.JobOffer;
using DTO.Notification.Search;
using DTO.Pagination;
using DTO.Sorting;
using DTO.User.Search;
using Elasticsearch.Net;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using System.Reflection;

namespace Infrastructure.Search;

public class ElasticSearchClient<T> : ISearchClient<T> where T : class, ISearchable
{
    private readonly IElasticClient _elasticClient;
    private readonly ISearchIndexProvider _searchIndexProvider;
    private readonly ILogger<ElasticSearchClient<T>> _logger;
    private readonly string _index;

    public ElasticSearchClient(
        IElasticClient elasticClient,
        ISearchIndexProvider searchIndexProvider,
        ILogger<ElasticSearchClient<T>> logger)
    {
        _elasticClient = elasticClient;
        _searchIndexProvider = searchIndexProvider;
        _logger = logger;
        _index = _searchIndexProvider.GetIndex<T>();
    }
    public async Task IndexAndRefreshAsync(T document, CancellationToken cancellationToken = default)
    {
        await _elasticClient.IndexAsync(document, descripor => descripor.Index(_index).Refresh(Refresh.WaitFor), cancellationToken);
        await _elasticClient.Indices.RefreshAsync(_index, ct: cancellationToken);
    }
    public async Task IndexAsync(T document, CancellationToken cancellationToken = default)
    {
        await _elasticClient.IndexAsync(document, descripor => descripor.Index(_index), cancellationToken);
    }
    public async Task IndexManyAsync(IEnumerable<T> data, CancellationToken cancellationToken = default)
    {
        await _elasticClient.IndexManyAsync(data, _index, cancellationToken);
    }
    public async Task IndexAndRefreshManyAsync(IEnumerable<T> data, CancellationToken cancellationToken = default)
    {
        await _elasticClient.IndexManyAsync(data, _index, cancellationToken);
        await _elasticClient.Indices.RefreshAsync(_index, ct: cancellationToken);
    }


    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _elasticClient.DeleteAsync<T>(id, descripor => descripor.Index(_index), cancellationToken);
    }

    public async Task DeleteAndRefreshAsync(int id, CancellationToken cancellationToken = default)
    {
        await _elasticClient.DeleteAsync<T>(id, descripor => descripor.Index(_index).Refresh(Refresh.WaitFor), cancellationToken);
        await _elasticClient.Indices.RefreshAsync(_index, ct: cancellationToken);
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        await _elasticClient.DeleteByQueryAsync<T>(q => q
            .Index(_index)
            .Query(rq => rq
                .MatchAll()
            ),
            cancellationToken);

        await _elasticClient.Indices.RefreshAsync(_index, ct: cancellationToken);
    }

    public async Task DeleteManyAsync(IEnumerable<T> data, CancellationToken cancellationToken = default)
    {
        await _elasticClient.DeleteManyAsync(data, _index, cancellationToken);
    }

    public async Task<PaginatedList<UserSearchable>> SearchUsersAsync(IUserFullSearchCriteria criteria)
    {
        var searchResponse = await _elasticClient.SearchAsync<UserSearchable>(s => s
            .Index(_index)
            .Query(q => BuildUserSearchQuery(q, criteria))
            .Sort(so => BuildUserSort(so, criteria.Sorting!))
            .From(Math.Max(0, (criteria.Paging.PageNumber - 1) * criteria.Paging.PageSize))
        .Size(Math.Max(1, criteria.Paging.PageSize)));

        return new PaginatedList<UserSearchable>(searchResponse.Documents.ToList(), (int)searchResponse.Total, criteria.Paging.PageNumber, criteria.Paging.PageSize);
    }

    public async Task<PaginatedList<NotificationSearchable>> SearchNotificationsForUserAsync(INotificationForUserFullSearchCriteria criteria)
    {
        var searchResponse = await _elasticClient.SearchAsync<NotificationSearchable>(s => s
            .Index(_index)
            .Query(q => BuildNotificationSearchQuery(q, criteria))
            .Sort(so => BuildNotificationSort(so, criteria.Sorting!))
            .From(Math.Max(0, (criteria.Paging.PageNumber - 1) * criteria.Paging.PageSize))
            .Size(Math.Max(1, criteria.Paging.PageSize)));

        return new PaginatedList<NotificationSearchable>(searchResponse.Documents.ToList(), (int)searchResponse.Total, criteria.Paging.PageNumber, criteria.Paging.PageSize);
    }

    public async Task<PaginatedList<JobOfferSearchable>> SearchJobOffersAsync(IJobOfferFullSearchCriteria criteria)
    {
        var searchResponse = await _elasticClient.SearchAsync<JobOfferSearchable>(s => s
            .Index(_index)
            .Query(q => BuildJobOfferSearchQuery(q, criteria))
            .Sort(so => BuildJobOfferSort(so, criteria.Sorting))
            .From(Math.Max(0, (criteria.Paging.PageNumber - 1) * criteria.Paging.PageSize))
            .Size(Math.Max(1, criteria.Paging.PageSize)));

        if (!searchResponse.IsValid)
            _logger.LogError("Elasticsearch job offer search failed: {DebugInformation}", searchResponse.DebugInformation);

        return new PaginatedList<JobOfferSearchable>(searchResponse.Documents.ToList(), (int)searchResponse.Total, criteria.Paging.PageNumber, criteria.Paging.PageSize);
    }

    public async Task<IReadOnlyList<JobOfferSearchable>> GetJobOffersByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        var response = await _elasticClient.SearchAsync<JobOfferSearchable>(s => s
            .Index(_index)
            .Query(q => q.Ids(i => i.Values(idList.Select(id => id.ToString()))))
            .Size(idList.Count), cancellationToken);

        if (!response.IsValid)
            _logger.LogError("Elasticsearch GetJobOffersByIds failed: {DebugInformation}", response.DebugInformation);

        return response.Documents.ToList();
    }

    public async Task DeleteIndexAsync(string index, CancellationToken cancellationToken = default)
    {
        var exists = await _elasticClient.Indices.ExistsAsync(index, ct: cancellationToken);
        if (exists.Exists)
            await _elasticClient.Indices.DeleteAsync(index, ct: cancellationToken);
    }

    private QueryContainer BuildUserSearchQuery(QueryContainerDescriptor<UserSearchable> descriptor, IUserFullSearchCriteria criteria)
    {
        var combinedQuery = new QueryContainer();

        if (!string.IsNullOrWhiteSpace(criteria.Query))
        {
            combinedQuery &= (BuildTextQuery(criteria.Query) ||
                              BuildWildcardQuery("firstName", criteria.Query) ||
                              BuildWildcardQuery("lastName", criteria.Query) ||
                              BuildWildcardQuery("email", criteria.Query) ||
                              BuildWildcardQuery("phoneNumber", criteria.Query));
        }

        if (criteria.Status.HasValue)
        {
            combinedQuery &= new TermQuery
            {
                Field = "status.id",
                Value = criteria.Status
            };
        }

        return combinedQuery;
    }

    private QueryContainer BuildNotificationSearchQuery(QueryContainerDescriptor<NotificationSearchable> descriptor, INotificationFullSearchCriteria criteria)
    {
        var combinedQuery = new QueryContainer();

        if (!string.IsNullOrWhiteSpace(criteria.Query))
        {
            combinedQuery &= (BuildTextQuery(criteria.Query) ||
                              BuildWildcardQuery("title", criteria.Query));
        }

        if (criteria is INotificationForUserFullSearchCriteria userCriteria)
        {
            combinedQuery &= new TermQuery
            {
                Field = "userId",
                Value = userCriteria.UserId
            };
        }

        if (criteria.Status.HasValue)
        {
            combinedQuery &= new TermQuery
            {
                Field = "status.id",
                Value = criteria.Status
            };
        }

        return combinedQuery;
    }

    private SortDescriptor<UserSearchable> BuildUserSort(SortDescriptor<UserSearchable> descriptor, SortOptions<UserFullSearchSortField> sortOptions)
    {
        if (sortOptions != null)
        {
            var sortOrder = sortOptions.SortOrder == DTO.Sorting.SortOrder.Asc ? Nest.SortOrder.Ascending : Nest.SortOrder.Descending;

            if (sortOptions.Field == UserFullSearchSortField.Status)
            {
                return descriptor.Field("status.id", sortOrder);
            }
            else
            {
                var fieldName = sortOptions.Field.ToString();

                var propertyInfo = FindProperty(typeof(UserSearchable), fieldName);

                if (propertyInfo != null)
                {
                    var lowerFieldName = char.ToLower(fieldName[0]) + fieldName.Substring(1); // Assuming enum values directly correspond to field names

                    // Check if the property is a string and append .keyword
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        lowerFieldName += ".keyword";
                    }

                    return descriptor.Field(lowerFieldName, sortOrder);
                }
                else
                {
                    throw new ArgumentException($"Property {fieldName} not found in type {typeof(UserSearchable)} or its base types.");
                }
            }
        }

        return descriptor;
    }

    private SortDescriptor<NotificationSearchable> BuildNotificationSort(SortDescriptor<NotificationSearchable> descriptor, SortOptions<NotificationFullSearchSortField> sortOptions)
    {
        if (sortOptions != null)
        {
            var sortOrder = sortOptions.SortOrder == DTO.Sorting.SortOrder.Asc ? Nest.SortOrder.Ascending : Nest.SortOrder.Descending;

            if (sortOptions.Field == NotificationFullSearchSortField.Status)
            {
                return descriptor.Field("status.id", sortOrder);
            }
            else
            {
                var fieldName = sortOptions.Field.ToString();

                var propertyInfo = FindProperty(typeof(NotificationSearchable), fieldName);

                if (propertyInfo != null)
                {
                    var lowerFieldName = char.ToLower(fieldName[0]) + fieldName.Substring(1); // Assuming enum values directly correspond to field names

                    // Check if the property is a string and append .keyword
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        lowerFieldName += ".keyword";
                    }

                    return descriptor.Field(lowerFieldName, sortOrder);
                }
                else
                {
                    throw new ArgumentException($"Property {fieldName} not found in type {typeof(NotificationSearchable)} or its base types.");
                }
            }
        }

        return descriptor;
    }

    private SortDescriptor<TDescriptor> BuildSort<TDescriptor, TSort>(SortDescriptor<TDescriptor> descriptor, SortOptions<TSort> sortOptions)
    where TDescriptor : class, ISearchable
    where TSort : Enum
    {
        if (sortOptions != null)
        {
            var fieldName = sortOptions.Field.ToString();

            var propertyInfo = FindProperty(typeof(TDescriptor), fieldName);

            if (propertyInfo != null)
            {
                var lowerFieldName = char.ToLower(fieldName[0]) + fieldName.Substring(1); // Assuming enum values directly correspond to field names
                var sortOrder = sortOptions.SortOrder == DTO.Sorting.SortOrder.Asc ? Nest.SortOrder.Ascending : Nest.SortOrder.Descending;

                // Check if the property is a string and append .keyword
                if (propertyInfo.PropertyType == typeof(string))
                {
                    lowerFieldName += ".keyword";
                }

                return descriptor.Field(lowerFieldName, sortOrder);
            }
            else
            {
                throw new ArgumentException($"Property {fieldName} not found in type {typeof(TDescriptor)} or its base types.");
            }
        }

        return descriptor;
    }

    private QueryContainer BuildJobOfferSearchQuery(QueryContainerDescriptor<JobOfferSearchable> descriptor, IJobOfferFullSearchCriteria criteria)
    {
        var combinedQuery = new QueryContainer();
        var hasFilters = false;

        if (!string.IsNullOrWhiteSpace(criteria.Query))
        {
            hasFilters = true;
            combinedQuery &= (BuildTextQuery(criteria.Query) ||
                              BuildWildcardQuery("title", criteria.Query) ||
                              BuildWildcardQuery("company", criteria.Query) ||
                              BuildWildcardQuery("industry", criteria.Query));
        }

        if (criteria.JobTypes is { Length: > 0 })
        {
            hasFilters = true;
            combinedQuery &= new TermsQuery
            {
                Field = "jobType.id",
                Terms = criteria.JobTypes.Select(jt => (object)(int)jt).ToList()
            };
        }

        if (criteria.WorkModel.HasValue)
        {
            hasFilters = true;
            combinedQuery &= new TermQuery
            {
                Field = "workModel.id",
                Value = (int)criteria.WorkModel.Value
            };
        }

        if (criteria.ExperienceLevel.HasValue)
        {
            hasFilters = true;
            combinedQuery &= new TermQuery
            {
                Field = "experienceLevel.id",
                Value = (int)criteria.ExperienceLevel.Value
            };
        }

        if (criteria.SalaryMin.HasValue)
        {
            hasFilters = true;
            combinedQuery &= new NumericRangeQuery
            {
                Field = "salaryMax",
                GreaterThanOrEqualTo = (double)criteria.SalaryMin.Value
            };
        }

        if (criteria.SalaryMax.HasValue)
        {
            hasFilters = true;
            combinedQuery &= new NumericRangeQuery
            {
                Field = "salaryMin",
                LessThanOrEqualTo = (double)criteria.SalaryMax.Value
            };
        }

        if (!string.IsNullOrWhiteSpace(criteria.Industry))
        {
            hasFilters = true;
            combinedQuery &= BuildWildcardQuery("industry", criteria.Industry);
        }

        if (criteria.JobFunctionIds is { Length: > 0 })
        {
            hasFilters = true;
            combinedQuery &= new TermsQuery
            {
                Field = "jobFunctions.id",
                Terms = criteria.JobFunctionIds.Select(id => (object)id).ToList()
            };
        }

        if (criteria.MinYearsOfExperience.HasValue || criteria.MaxYearsOfExperience.HasValue)
        {
            hasFilters = true;
            combinedQuery &= new NumericRangeQuery
            {
                Field = "yearsRequired",
                GreaterThanOrEqualTo = criteria.MinYearsOfExperience.HasValue ? (double)criteria.MinYearsOfExperience.Value : null,
                LessThanOrEqualTo = criteria.MaxYearsOfExperience.HasValue ? (double)criteria.MaxYearsOfExperience.Value : null
            };
        }

        if (!string.IsNullOrWhiteSpace(criteria.Location))
        {
            hasFilters = true;
            combinedQuery &= BuildWildcardQuery("location", criteria.Location);
        }

        if (criteria.Skills is { Length: > 0 })
        {
            hasFilters = true;
            var skillsQuery = criteria.Skills
                .Aggregate(new QueryContainer(), (acc, skill) => acc | BuildWildcardQuery("requiredSkills", skill));
            combinedQuery &= skillsQuery;
        }

        if (criteria.PostedAfter.HasValue || criteria.PostedBefore.HasValue)
        {
            hasFilters = true;
            combinedQuery &= new DateRangeQuery
            {
                Field = "postedAt",
                GreaterThanOrEqualTo = criteria.PostedAfter,
                LessThanOrEqualTo = criteria.PostedBefore
            };
        }

        if (criteria.IncludeIds is { Length: > 0 })
        {
            hasFilters = true;
            combinedQuery &= new IdsQuery { Values = criteria.IncludeIds.Select(id => (Id)id.ToString()).ToList() };
        }

        if (criteria.ExcludeIds is { Length: > 0 })
        {
            hasFilters = true;
            combinedQuery &= !new IdsQuery { Values = criteria.ExcludeIds.Select(id => (Id)id.ToString()).ToList() };
        }

        return hasFilters ? combinedQuery : descriptor.MatchAll();
    }

    private SortDescriptor<JobOfferSearchable> BuildJobOfferSort(SortDescriptor<JobOfferSearchable> descriptor, SortOptions<JobOfferFeedSortField>? sortOptions)
    {
        if (sortOptions == null)
            return descriptor.Descending("postedAt");

        var sortOrder = sortOptions.SortOrder == DTO.Sorting.SortOrder.Asc ? Nest.SortOrder.Ascending : Nest.SortOrder.Descending;

        return sortOptions.Field switch
        {
            JobOfferFeedSortField.Match => descriptor.Field("matchScore", sortOrder),
            JobOfferFeedSortField.Date => descriptor.Field("postedAt", sortOrder),
            _ => descriptor.Descending("postedAt")
        };
    }

    private QueryContainer BuildTextQuery(string query)
    {
        return new QueryStringQuery
        {
            Query = query
        };
    }

    private QueryContainer BuildWildcardQuery(string field, string query)
    {
        return new WildcardQuery
        {
            Field = field, // Field to search
            Value = "*" + query.ToLowerInvariant() + "*", // Add wildcards to both sides of the query term
            Rewrite = MultiTermQueryRewrite.ConstantScore // Optional: Set the rewrite method
        };
    }
    private PropertyInfo FindProperty(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName);
        if (property != null)
        {
            return property;
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            property = interfaceType.GetProperty(propertyName);
            if (property != null)
            {
                return property;
            }
        }

        if (type.BaseType != null && type.BaseType != typeof(object))
        {
            return FindProperty(type.BaseType, propertyName);
        }

        return null;
    }

    public async Task<bool> IndexExist(string index)
    {
        try
        {
            var response = await _elasticClient.Indices.ExistsAsync(new IndexExistsRequest(index));
            return response.Exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while executing IndexExist");
            return false;
        }
    }

    public async Task<bool> CreateIndexIfNotExist(string index)
    {
        const int maxRetryAttempts = 10;
        const int delayBetweenRetries = 6000;

        try
        {
            for (int attempt = 1; attempt <= maxRetryAttempts; attempt++)
            {
                if (await IsElasticsearchAvailable())
                {
                    _logger.LogDebug("Connected to Elasticsearch after {0} attempt(s)", attempt);
                    break;
                }

                _logger.LogWarning("Elasticsearch not available. Retrying {0}/{1}...", attempt, maxRetryAttempts);

                if (attempt == maxRetryAttempts)
                {
                    _logger.LogError("Failed to connect to Elasticsearch after {0} attempts", maxRetryAttempts);
                    return false;
                }

                await Task.Delay(delayBetweenRetries);
            }

            var indexExistsResponse = await _elasticClient.Indices.ExistsAsync(index);
            if (!indexExistsResponse.Exists)
            {
                _logger.LogDebug("Creating index {0}", index);
                var createIndexResponse = await _elasticClient.Indices.CreateAsync(index, cid => cid.Map<T>(m => m.AutoMap()));
                if (!createIndexResponse.IsValid)
                {
                    _logger.LogDebug("Failed to create index {0}. Response: {1}", index, JsonConvert.SerializeObject(createIndexResponse));
                    return false;
                }
                else
                {
                    _logger.LogDebug("Index {0} created", index);
                    return true;
                }
            }

            _logger.LogDebug("Creating index {0} skipped. Already exist", index);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating index {0}", index);
            return false;
        }
    }
    private async Task<bool> IsElasticsearchAvailable()
    {
        try
        {
            var pingResponse = await _elasticClient.PingAsync();
            return pingResponse.IsValid;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ping to Elasticsearch failed.");
            return false;
        }
    }
}
