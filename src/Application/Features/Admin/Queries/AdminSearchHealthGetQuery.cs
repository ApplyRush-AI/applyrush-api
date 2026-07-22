using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Search;
using Application.Features.JobOffers.Search;

namespace Application.Features.Admin.Queries;

public sealed record AdminSearchHealthResponse(
    bool ElasticsearchReachable,
    bool JobOfferIndexExists,
    string? Error
);

public sealed record AdminSearchHealthGetQuery : IQuery<AdminSearchHealthResponse>;

public sealed class AdminSearchHealthGetQueryHandler : IQueryHandler<AdminSearchHealthGetQuery, AdminSearchHealthResponse>
{
    private readonly ISearchClient<JobOfferSearchable> _searchClient;
    private readonly ISearchIndexProvider _searchIndexProvider;

    public AdminSearchHealthGetQueryHandler(
        ISearchClient<JobOfferSearchable> searchClient,
        ISearchIndexProvider searchIndexProvider)
    {
        _searchClient = searchClient;
        _searchIndexProvider = searchIndexProvider;
    }

    public async Task<AdminSearchHealthResponse> Handle(AdminSearchHealthGetQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var index = _searchIndexProvider.GetIndex<JobOfferSearchable>();

            // Ping first: IndexExist catches its own exceptions and returns false, so on its own it would
            // report a dead cluster as "reachable, index missing" - which is exactly how this was misread.
            var reachable = await _searchClient.IsAvailableAsync();
            if (!reachable)
                return new AdminSearchHealthResponse(false, false, "Elasticsearch did not respond to a ping. Job search will return no results until the cluster is reachable.");

            var indexExists = await _searchClient.IndexExist(index);
            var error = indexExists
                ? null
                : $"Elasticsearch is reachable but the '{index}' index does not exist. Run POST /Admin/search/reindex to rebuild it.";

            return new AdminSearchHealthResponse(true, indexExists, error);
        }
        catch (Exception ex)
        {
            return new AdminSearchHealthResponse(false, false, ex.Message);
        }
    }
}
