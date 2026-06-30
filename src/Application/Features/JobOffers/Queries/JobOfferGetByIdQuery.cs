using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.Jobs.UserJobMatches;
using DTO.JobOffers;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobOffers.Queries;

public sealed record JobOfferGetByIdQuery(int JobId) : IQuery<JobOfferDetailResponse>;

public sealed class JobOfferGetByIdQueryHandler : IQueryHandler<JobOfferGetByIdQuery, JobOfferDetailResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IMatchScoringService _matchScoringService;

    public JobOfferGetByIdQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper,
        IMatchScoringService matchScoringService)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
        _matchScoringService = matchScoringService;
    }

    public async Task<JobOfferDetailResponse> Handle(JobOfferGetByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        IQueryable<JobListing> query = _dbContext.JobListing
            .AsNoTracking()
            .Include(j => j.JobFunctions)
                .ThenInclude(jf => jf.JobFunction);

        if (userId.HasValue)
        {
            query = query
                .Include(j => j.UserMatches.Where(m => m.UserId == userId.Value))
                .Include(j => j.SavedByUsers.Where(s => s.UserId == userId.Value));
        }

        var job = await query
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken)
            ?? throw NotFoundException.New<JobListing>();

        if (userId.HasValue && !job.UserMatches.Any())
            await _matchScoringService.ComputeAndSaveAsync(userId.Value, job.Id, cancellationToken);

        return _mapper.Map<JobOfferDetailResponse>(job);
    }
}

