using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using AutoMapper;
using DTO.Enums;
using DTO.JobOffers;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.JobOffers.Queries;

public sealed record SavedJobOfferListGetQuery : IQuery<SavedJobOfferListResponse>;

public sealed class SavedJobOfferListGetQueryHandler : IQueryHandler<SavedJobOfferListGetQuery, SavedJobOfferListResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public SavedJobOfferListGetQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMapper mapper)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<SavedJobOfferListResponse> Handle(SavedJobOfferListGetQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var savedJobs = await _dbContext.UserSavedJob
            .AsNoTracking()
            .Where(s => s.UserId == userId && s.Status == Status.Active)
            .Include(s => s.Job)
                .ThenInclude(j => j.UserMatches.Where(m => m.UserId == userId))
            .OrderByDescending(s => s.Created)
            .ToListAsync(cancellationToken);

        var jobs = savedJobs.Select(s => s.Job).ToList();
        var savedJobIds = savedJobs.Select(s => s.JobId).ToList();
        var mappedJobs = _mapper.Map<List<JobOfferFeedItemResponse>>(jobs)
            .Select(j => j with { IsSaved = true })
            .ToList();

        return new SavedJobOfferListResponse
        {
            SavedJobIds = savedJobIds,
            Jobs = mappedJobs
        };
    }
}
