using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.JobOffers;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobOffers.Queries;

public sealed record JobOfferGetAllQuery : IQuery<IReadOnlyCollection<JobOfferFeedItemResponse>>;

public sealed class JobOfferGetAllQueryHandler : IQueryHandler<JobOfferGetAllQuery, IReadOnlyCollection<JobOfferFeedItemResponse>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public JobOfferGetAllQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<JobOfferFeedItemResponse>> Handle(JobOfferGetAllQuery request, CancellationToken cancellationToken)
    {
        var jobs = await _dbContext.JobListing
            .AsNoTracking()
            .Include(j => j.JobFunctions)
                .ThenInclude(jf => jf.JobFunction)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyCollection<JobOfferFeedItemResponse>>(jobs);
    }
}
