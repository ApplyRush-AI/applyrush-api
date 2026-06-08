using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;

namespace Application.Features.Cities.Queries;

public sealed record CityListGetQuery(string? Search, int? Limit) : IQuery<IReadOnlyList<string>>;

public sealed class CityListGetQueryHandler : IQueryHandler<CityListGetQuery, IReadOnlyList<string>>
{
    private readonly ICityProvider _cityProvider;

    public CityListGetQueryHandler(ICityProvider cityProvider)
    {
        _cityProvider = cityProvider;
    }

    public Task<IReadOnlyList<string>> Handle(CityListGetQuery query, CancellationToken cancellationToken)
    {
        var result = _cityProvider.Search(query.Search, query.Limit ?? 20);
        return Task.FromResult(result);
    }
}
