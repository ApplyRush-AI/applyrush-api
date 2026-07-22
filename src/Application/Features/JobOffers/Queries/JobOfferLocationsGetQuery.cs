using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using DTO.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobOffers.Queries;

public sealed record JobOfferLocationsGetQuery : IQuery<IReadOnlyList<string>>;

public sealed class JobOfferLocationsGetQueryHandler : IQueryHandler<JobOfferLocationsGetQuery, IReadOnlyList<string>>
{
    private readonly IApplicationDbContext _dbContext;

    public JobOfferLocationsGetQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Mirrors the industries endpoint: the client picks from these exact values, so what it sends back
    // is guaranteed to match how locations are stored ("City, State, Country").
    public async Task<IReadOnlyList<string>> Handle(JobOfferLocationsGetQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.JobListing
            .AsNoTracking()
            .Where(j => j.Status == Status.Active && j.Location != null && j.Location != "")
            .Select(j => j.Location)
            .Distinct()
            .OrderBy(l => l)
            .ToListAsync(cancellationToken);
    }
}
