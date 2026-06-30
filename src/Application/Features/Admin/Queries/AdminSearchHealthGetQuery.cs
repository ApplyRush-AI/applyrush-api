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
            var indexExists = await _searchClient.IndexExist(index);
            return new AdminSearchHealthResponse(true, indexExists, null);
        }
        catch (Exception ex)
        {
            return new AdminSearchHealthResponse(false, false, ex.Message);
        }
    }
}
