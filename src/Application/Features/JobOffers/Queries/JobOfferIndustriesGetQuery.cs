using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobOffers.Queries;

public sealed record JobOfferIndustriesGetQuery : IQuery<IReadOnlyList<string>>;

public sealed class JobOfferIndustriesGetQueryHandler : IQueryHandler<JobOfferIndustriesGetQuery, IReadOnlyList<string>>
{
    private readonly IApplicationDbContext _dbContext;

    public JobOfferIndustriesGetQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<string>> Handle(JobOfferIndustriesGetQuery query, CancellationToken cancellationToken)
    {
        return await _dbContext.JobListing
            .AsNoTracking()
            .Where(j => j.Industry != null)
            .Select(j => j.Industry!)
            .Distinct()
            .OrderBy(i => i)
            .ToListAsync(cancellationToken);
    }
}
